namespace Argot.XMPP.Core.SASL.Extensions;

public static class ExtensionMethods
{
    public static string EnumValue(this SASLMechanismType mechanism) =>
        mechanism switch
        {
            SASLMechanismType.Plain => "PLAIN",
            SASLMechanismType.DigestMD5 => "DIGEST-MD5",
            SASLMechanismType.SCRAM => "SCRAM-SHA-1",
            _ => "UNKNOWN"
        };
}
