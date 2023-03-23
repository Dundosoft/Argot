using Argot.Common.Settings;
using Argot.XMPP.Core.Stanzas;
using System.Xml.Linq;

namespace Argot.XMPP.Core;


public abstract class XMPPConnection : IDisposable
{
    private readonly string _ns;
    public Account Account { get; protected set; }
    public string Namespace => _ns;
    public XMPPConnection(string ns)
    {
        _ns = ns;
    }

    public abstract XElement NextElement();
    public virtual void Send(XElement element) => OnElement(new() { Element = element });


    public abstract Task ConnectAsync(Guid accountId, CancellationToken cancellationToken);
    public abstract void Disconnect();

    #region Handlers

    public delegate void RequestCredentialsHandler(XMPPConnection connection, CredentialArgs args);
    public event RequestCredentialsHandler RequestCredentials = delegate { };
    public void OnRequestCredentials(XMPPConnection connection, CredentialArgs args) => RequestCredentials.Invoke(connection, args);

    public delegate void ConnectionFailedHandler(XMPPConnection connection, ConnectionFailedArgs args);
    public event ConnectionFailedHandler ConnectionFailed = delegate { };
    protected void OnConnectionFailed(ConnectionFailedArgs args) => ConnectionFailed.Invoke(this, args);

    public delegate void InitiateStreamHandler(XMPPConnection connection, string streamId);
    public event InitiateStreamHandler InitiateStream = delegate { };
    protected void OnInitiateStream(string streamId) => InitiateStream.Invoke(this, streamId);

    public delegate void AuthenticatedHandler(XMPPConnection connection, AuthenticatedArgs args);
    public event AuthenticatedHandler Authenticated = delegate { };
    protected void OnSignedIn(AuthenticatedArgs args) => Authenticated.Invoke(this, args);

    public delegate void ElementHandler(XMPPConnection connection, ElementArgs args);
    public event ElementHandler Element = delegate { };
    protected void OnElement(ElementArgs args) => Element.Invoke(this, args);

    public delegate void IQHandler(XMPPConnection connection, IQ arg);
    protected event IQHandler IQ = delegate { };
    protected void OnIq(IQ iq) => IQ.Invoke(this, iq);

    public delegate void MessageHandler(XMPPConnection connection, Message message);
    public event MessageHandler Message = delegate { };
    protected void OnMessage(Message message) => Message.Invoke(this, message);

    public delegate void PresenceHandler(XMPPConnection connection, Presence presence);
    public event PresenceHandler Presence = delegate { };
    protected void OnPresence(Presence e) => Presence.Invoke(this, e);

    #endregion

    public virtual void Dispose()
    {
    }
}

#region Handler Args


public class CredentialArgs
{
    public string Password { get; set; }
}

public class ConnectionFailedArgs
{
    public Exception Exception { get; set; }
    public string Message { get; set; }
}

public class ElementArgs
{
    public XElement Element { get; set; }
}

public class AuthenticatedArgs
{
    public JID JID { get; set; }
}

#endregion
