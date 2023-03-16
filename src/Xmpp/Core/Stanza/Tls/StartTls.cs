using System.Xml.Linq;
using Xmpp.Core.Stanza.Attributes;

namespace Xmpp.Core.Stanza
{
    [XmppElement("starttls", Namespaces.Tls, typeof(StartTls))]
    public class StartTls : XmppElement
    {
        public StartTls(XElement other)
            : base(other)
        {
        }

        public StartTls()
            : base(ElementName)
        {
        }

        public static XName ElementName => XName.Get("starttls", Namespaces.Tls);

        public bool Required => Element<Required>(XName.Get("required", Namespaces.Tls)) is not null;
    }
}
