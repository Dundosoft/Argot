using Microsoft.Extensions.Logging;
using System.Net;
using Ubiety.Dns.Core;
using Ubiety.Dns.Core.Common;
using Ubiety.Dns.Core.Records;
using Ubiety.Dns.Core.Records.General;

namespace Xmpp.Core.Net
{
    /// <summary>
    ///     Address class
    /// </summary>
    internal class Address
    {

        private readonly ILogger<Address> _logger;
        private readonly Resolver _resolver;

        public Address(Jid jid, string hostname = null)
        {
            _resolver = ResolverBuilder.Begin()
                .AddDnsServer("8.8.8.8")
                .SetTimeout(1000)
                .EnableCache()
                .SetRetries(3)
                .Build();

            Hostname = hostname is null ? jid.Hostname : hostname;
        }

        /// <summary>
        ///     Gets a value indicating whether the address is IPv6
        /// </summary>
        public bool IsIPv6 { get; private set; }

        /// <summary>
        ///     Gets the hostname of the address
        /// </summary>
        public string Hostname { get; private set; }

        public IPAddress IPAddress => GetIPAddress();

        /// <summary>
        ///     Gets the next IP address for the server
        /// </summary>
        /// <returns><see cref="IPAddress" /> of the XMPP server</returns>
        public IPAddress GetIPAddress()
        {
            var srvRecords = ResolveSrv();

            if ( srvRecords.Any() )
            {
                Hostname = srvRecords.Select(srv => srv.Target).First();
                return Resolve();
            }

            return Resolve();
        }

        // TODO: handle ipv6?
        private IPAddress Resolve()
        {
            var response = _resolver.Query(Hostname, QuestionType.AAAA, QuestionClass.IN);

            return response.Answers
                .Select(answer => answer.Record).OfType<RecordA>()
                .Select(a => a.Address)
                .FirstOrDefault();
        }

        private IEnumerable<RecordSrv> ResolveSrv()
        {
            var response = _resolver.Query($"_xmpp-client._tcp.{Hostname}", QuestionType.SRV, QuestionClass.IN);

            return response.Header.AnswerCount > 0
                ?  response.Answers.Select(record => record.Record as RecordSrv)
                : Enumerable.Empty<RecordSrv>();
        }
    }
}
