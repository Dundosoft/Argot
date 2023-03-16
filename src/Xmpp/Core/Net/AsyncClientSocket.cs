using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;
using Xmpp.Core.Stanza;

namespace Xmpp.Core.Net
{
    public class AsyncClientSocket : ISocket, IDisposable
    {
        private const int BufferSize = 4 *1024;
        private readonly AutoResetEvent _resetEvent;
        private readonly UTF8Encoding _utf8 = new ();
        private Address _address;
        private Socket _socket;
        private Stream _stream;

        public AsyncClientSocket()
        {
            _resetEvent = new AutoResetEvent(false);
        }

        public bool Connected { get; private set; }
        public bool Secure { get; private set; }

        public event EventHandler ConnectionEventHandler;
        public event EventHandler<StanzaEventArgs> StanzaEventHandler;

        public void Disconnect()
        {
            Connected = false;
            _stream.Close();
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Disconnect(true);
        }

        public bool Connect(Jid jid, string hostname = null, int port = 5222)
        {
            _address = new Address(jid, hostname);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var args = new SocketAsyncEventArgs();

            args.Completed += ConnectCompleted;
            args.RemoteEndPoint = new IPEndPoint(_address.GetIPAddress(), port);

            try
            {
                var completed = _socket.ConnectAsync(args);


                if (completed)
                {
                    return false;
                }

                ConnectCompleted(this, args);
                return true;
            }
            catch(SocketException exception)
            {
                return false;
            }
        }

        public void Send(string xmlString)
        {
            if(!Connected)
            {
                return;
            }

            var bytes = Encoding.UTF8.GetBytes(xmlString);
            _stream.WriteAsync(bytes, 0, bytes.Length);
        }
        public void Send(Stanza.Stanza stanza) => Send(stanza.ToString());

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _socket?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private Task<XElement> ReadStanza()
        {
            _resetEvent.WaitOne();
            var buffer = new byte[BufferSize];
            var received = _stream.ReadAsync(buffer, 0, BufferSize);

            return received.ContinueWith(_ =>
            {
                Array.Resize(ref buffer, received.Result);

                return XElement.Parse(_utf8.GetString(buffer));
            });
        }

        protected virtual void OnStanzaEvent(StanzaEventArgs stanzaEventArgs) =>
            StanzaEventHandler?.Invoke(this, stanzaEventArgs);

        protected virtual void OnConnectionEvent() =>
            ConnectionEventHandler?.Invoke(this, new EventArgs());

        private void ConnectCompleted(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            if (socketAsyncEventArgs.SocketError is not SocketError.Success)
            {
                return;
            }

            var socket = socketAsyncEventArgs.ConnectSocket;
            _stream = new NetworkStream(socket);

            Connected = true;
            OnConnectionEvent();

            ReadStanzaLoop();
        }

        private async void ReadStanzaLoop()
        {
            while (Connected)
            {
                var stanza = await ReadStanza();
                OnStanzaEvent(new StanzaEventArgs { Stanza = stanza });
            }
        }

    }
}
