using System.Text;

namespace Argot.XMPP.Core.SASL;

public class SASLPlainHandler : SASLHandler
{
    public override SASLMechanismType SASLMechanismType => SASLMechanismType.Plain;
    public override string Initiate() =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes($"{JID.BareJid}\0{JID.Local}\0{Password}"));
    public override string NextChallenge(string previousResponse) => string.Empty;
}
