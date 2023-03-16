namespace Xmpp.Core.Net
{
    public interface IXmppClient
    {

    }

    public class XmppClient : IXmppClient
    {
        private readonly AsyncClientSocket _socket;
        public XmppClient()
        {
            _socket = new AsyncClientSocket();
        }

        public bool Connect(Jid jid, string hostname, int port)
        {
//            _logger.LogDebug("Client connecting: {jid}, {hostname}, {port}", jid, hostname, port);
            return true;
        }
    }
}
