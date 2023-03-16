using Xmpp.Core.Stanza;

namespace Xmpp.Core.Net
{
    public interface ISocket
    {
        event EventHandler<StanzaEventArgs> StanzaEventHandler;

        event EventHandler ConnectionEventHandler;

        bool Connected { get; }

        bool Connect(Jid jid, string hostname = null, int port = 5222);

        void Disconnect();

        void Send(Stanza.Stanza stanza);
    }
}
