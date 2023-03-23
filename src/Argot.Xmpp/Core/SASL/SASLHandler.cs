using Argot.XMPP.Core.SASL.Extensions;
using Argot.XMPP.Core.Stanzas.SASL;

namespace Argot.XMPP.Core.SASL;

public abstract class SASLHandler
{
    public abstract SASLMechanismType SASLMechanismType { get; }
    public JID JID { get; set; }
    public string Password { get; set; }
    public abstract string Initiate();
    public abstract string NextChallenge(string previousResponse);

    public void Start(XMPPConnection connection)
    {
        var auth = new Auth();
        auth.SetAttributeValue("mechanism", SASLMechanismType.EnumValue());
        auth.SetValue(Initiate());
        connection.Send(auth);
        var authResponse = connection.NextElement();
        var nextResponse = string.Empty;
        while ((nextResponse = NextChallenge(authResponse.Value)) != "")
        {
            if (nextResponse == "error")
            {
                OnAuthenticationFailed(connection);
                return;
            }
            var response = new Response();
            response.SetValue(nextResponse);
            connection.Send(response);
            authResponse = connection.NextElement();
        }
        OnAuthenticated(connection);
    }


    public static SASLHandler Create(IEnumerable<SASLMechanismType> availableMechanisms, JID clientJID, string password) =>
        availableMechanisms switch
        {
            var available when available.Contains(SASLMechanismType.Plain) => new SASLPlainHandler(),
            var available when available.Contains(SASLMechanismType.DigestMD5) => new SASLDigestMd5(),
            var available when available.Contains(SASLMechanismType.SCRAM) => new SASLSCRAM(),
            _ => null
        };

    public delegate void AuthenticatedHandler(XMPPConnection connection);
    public event AuthenticatedHandler Authenticated = delegate { };
    protected virtual void OnAuthenticated(XMPPConnection connection) => Authenticated.Invoke(connection);

    public delegate void AuthenticationFailedHandler(XMPPConnection connection);
    public event AuthenticationFailedHandler AuthenticationFailed = delegate { };
    protected virtual void OnAuthenticationFailed(XMPPConnection connection) => AuthenticationFailed.Invoke(connection);


}
