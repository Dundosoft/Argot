using System.Xml.Linq;

namespace Argot.XMPP.Core.Stanzas.SASL;

public class Challenge : XElement
{
    public Challenge() : base (XNamespace.Get(Namespaces.XmppSasl) + "challenge")
    {
    }
}
