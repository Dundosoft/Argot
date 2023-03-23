using Argot.Common.Settings;
using Argot.XMPP.Core.SASL;
using Argot.XMPP.Core.Stanzas.Stream;
using Argot.XMPP.Core.Stanzas.TLS;
using DnsClient;
using DnsClient.Protocol;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Xml;
using System.Xml.Linq;

namespace Argot.XMPP.Core
{
    public class XMPPTCPConnection : XMPPConnection
    {
        private object _terminateLock = new ();
        private bool _isDisposed;
        private TcpClient _tcpClient;
        private ILookupClient _lookupClient;
        private ISettingsService _settingsService;

        protected virtual int TCPPort { get; set; }
        protected XmlReader Reader { get; set; }
        protected XmlWriter Writer { get; set; }

        public XMPPTCPConnection(
            string ns,
            ILookupClient lookupClient,
            ISettingsService settingsService) 
                : base(ns)
        {
            _lookupClient = lookupClient;
            _settingsService = settingsService;
        }

        public System.IO.Stream ConnectionStream { get; private protected set; }

        public override async Task ConnectAsync(Guid accountid, CancellationToken token)
        {
            Account = _settingsService.GetAccount(accountid);

            var addresses = await ResolveHostAddresses(token);
            await ConnectOverTcp(addresses, token);

            StreamInitiate();

            var features = ParseFeatures();

            var tlsSupported = await InitTLS(features, token);
            if (tlsSupported)
            {
                features = ParseFeatures();
            }

            throw new NotImplementedException();
        }

        private Task InitializeAuthentication(Features features, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            var jid = new JID(Account.JID);

            void RunCatching(Action act)
            {
                try
                {
                    act();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }

            Task.Run(() => RunCatching(() =>
            {
                var authenticator = SASLHandler.Create(features.SASLMechanisms, jid, Account.Password);
                if (authenticator == null)
                {
                    OnConnectionFailed(new ConnectionFailedArgs { Message = "supported sasl mechanism not available" });
                    tcs.TrySetResult(false);
                    return;
                }
                authenticator.Authenticated += _ => RunCatching(() =>
                {
                    StreamInitiate();
                    /*
                    var session = new SessionHandler();
                    session.SessionStarted += connection => RunCatching(() =>
                    {
                        OnSignedIn(new SignedInArgs { Jid = connection.Jid });
                        tcs.TrySetResult(true);
                    });
                    // TODO make async
                    // Locks stream with SessionLoop
                    session.Start(this);
                    */
                });
                authenticator.AuthenticationFailed += _ => RunCatching(() =>
                {
                    OnConnectionFailed(new ConnectionFailedArgs { Message = "Authentication failed" });
                    tcs.TrySetResult(true);
                });

                using (cancellationToken.Register(TerminateTcpConnection))
                    authenticator.Start(this);

                tcs.TrySetResult(false);
            }), cancellationToken);
            return tcs.Task;
        }

        protected void StreamInitiate()
        {
            var writerSettings = new XmlWriterSettings
            {
                ConformanceLevel = ConformanceLevel.Fragment,
                OmitXmlDeclaration = true
            };
            var readerSettings = new XmlReaderSettings
            {
                ConformanceLevel = ConformanceLevel.Fragment,
            };

            Writer = XmlWriter.Create(ConnectionStream, writerSettings);
            Reader = XmlReader.Create(ConnectionStream, readerSettings);
        }

        protected void WriteStreamInitiate()
        {
            Writer.WriteStartElement("stream", "stream", Namespaces.Streams);
            Writer.WriteAttributeString("xmlns", Namespace);
            Writer.WriteAttributeString("version", "1.0");
            // TODO check if we should force hostname
            Writer.WriteAttributeString("to", new JID(Account.JID).Domain);
            Writer.WriteRaw("");
            Writer.Flush();
        }

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        public override XElement NextElement()
        {
            throw new NotImplementedException();
        }

        public override void Send(XElement element)
        {
            base.Send(element);
        }

        private Features ParseFeatures() => Stanza.Parse<Features>(NextElement());

        private async Task ConnectOverTcp(IEnumerable<IPAddress> addresses, CancellationToken cancellationToken)
        {
            TerminateTcpConnection();

            _tcpClient = new();

            try
            {
                await _tcpClient.ConnectAsync(addresses.ToArray(), TCPPort);
                ConnectionStream = _tcpClient.GetStream();
            }
            catch
            {
                ((IDisposable)_tcpClient).Dispose();
                _tcpClient = null;
                throw;
            }
        }

        private async Task<IEnumerable<IPAddress>> ResolveHostAddresses(CancellationToken cancellationToken)
        {
            var jid = new JID(Account.JID);
            List<IPAddress> hostAddresses = new();
            var queryResponse = await _lookupClient.QueryAsync($"_xmpp-client._tcp.{jid.Domain}", QueryType.SRV, cancellationToken: cancellationToken);
            var targets = queryResponse.Answers.OfType<SrvRecord>().Select(srvRecord => srvRecord.Target);

            if (targets.Any())
            {
                foreach ( var target in targets) {
                    var addresses = await _lookupClient.QueryAsync(target.Value, QueryType.A, cancellationToken: cancellationToken);
                    hostAddresses.AddRange(addresses.Answers.ARecords().Select(x => x.Address));
                }
            }
            else
            {
                var addresses = await _lookupClient.QueryAsync(jid.Domain, QueryType.A, cancellationToken: cancellationToken);
                hostAddresses.AddRange(addresses.Answers.ARecords().Select(x => x.Address));
            }

            return hostAddresses;
        }

        private async Task<bool>  InitTLS(Features features, CancellationToken cancellationToken)
        {
            var jid = new JID(Account.JID);

            if (!features.StartTLS) return false;

            Send(new StartTLS());

            var tlsResponse = Stanza.Parse<Proceed>(NextElement());

            if (tlsResponse is null) return false;

            ConnectionStream = new SslStream(ConnectionStream, true);

            var sslOptions = new SslClientAuthenticationOptions
            {
                TargetHost = jid.Domain,
            };

            await (ConnectionStream as SslStream).AuthenticateAsClientAsync(sslOptions, cancellationToken);

            StreamInitiate();

            return true;
        }

        private void TerminateTcpConnection()
        {
            // There are three callers for this method: connection timeout, external disconnect, and external dispose.
            // This lock is placed in case of a race condition between the two.
            lock (_terminateLock)
            {
                Writer?.Dispose();
                Writer = null;
                Reader?.Dispose();
                Reader = null;
                ConnectionStream?.Dispose();
                ConnectionStream = null;
                _tcpClient = null;
            }
        }

        public override void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                TerminateTcpConnection();
            }
            base.Dispose();
        }
    }
}
