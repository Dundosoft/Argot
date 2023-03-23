using System.Xml.Linq;

namespace Argot.XMPP.Core.Stanzas.SASL;

public class Response : XElement
{
    public Response() : base(XNamespace.Get(Namespaces.XmppSasl) + "response")
    {
    }
}
