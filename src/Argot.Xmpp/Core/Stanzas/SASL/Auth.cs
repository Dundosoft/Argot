using System.Xml.Linq;

namespace Argot.XMPP.Core.Stanzas.SASL;

public class Auth : XElement
{
    public Auth() : base(XNamespace.Get(Namespaces.XmppSasl) + "auth")
        { }
}
