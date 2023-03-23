using System.Xml.Linq;

namespace Argot.XMPP.Core.Stanzas.SASL;

public class Success : XElement
{
    public Success() : base(XNamespace.Get(Namespaces.XmppSasl) + "success")
    { 
    }
}
