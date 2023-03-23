using System.Xml.Linq;

namespace Argot.XMPP.Core.Stanzas.SASL;

public class Failure : XElement
{
    public Failure() : base(XNamespace.Get(Namespaces.XmppSasl) + "failure")
    {
    }
}
