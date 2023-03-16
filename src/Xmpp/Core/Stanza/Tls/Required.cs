using System.Xml.Linq;
using Xmpp.Core.Stanza.Attributes;

namespace Xmpp.Core.Stanza
{
    [XmppElement("required", Namespaces.Tls, typeof(Required))]
    public class Required : XmppElement
    {
        public Required(XElement other)
            : base(other)
        {
        }

        public Required()
            : base(ElementName)
        {
        }

        public static XName ElementName => XName.Get("required", Namespaces.Tls);
    }
}
