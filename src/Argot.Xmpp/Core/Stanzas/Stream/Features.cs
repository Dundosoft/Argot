using Argot.XMPP.Core.SASL;
using System.Xml.Linq;

namespace Argot.XMPP.Core.Stanzas.Stream;

public class Features : Stanza
{
    public Features() : base(XNamespace.Get(Namespaces.Streams) + "features")
    {

    }

    public bool StartTLS => Element(XNamespace.Get(Namespaces.XmppTls) + "starttls") is not null;
    public bool Bind => Element(XNamespace.Get(Namespaces.XmppBind) + "bind") is not null;
    public bool Session => Element(XNamespace.Get(Namespaces.XmppSession) + "session") is not null;
    public IEnumerable<SASLMechanismType> SASLMechanisms =>
        Element(XNamespace.Get(Namespaces.XmppSasl) + "mechanisms") is XElement saslMechanisms
            ? saslMechanisms
                .Elements(XNamespace.Get(Namespaces.XmppSasl) + "mechanism")
                .Select(mechanism => mechanism.Value switch
                    {
                        "PLAIN" => SASLMechanismType.Plain,
                        "DIGEST-MD5" => SASLMechanismType.DigestMD5,
                        "EXTERNAL" => SASLMechanismType.External,
                        "SCRAM-SHA-1" => SASLMechanismType.SCRAM,
                        "SCRAM-SHA-1-PLUS" => SASLMechanismType.SCRAMPlus,
                        _ => SASLMechanismType.NotSupported
                    })
            : Enumerable.Empty<SASLMechanismType>();
}
