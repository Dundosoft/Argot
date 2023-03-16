using System.Xml.Linq;
using Xmpp.Core.Stanza.Attributes;

namespace Xmpp.Core.Stanza
{
    [XmppElement("proceed", Namespaces.Tls, typeof(Proceed))]
    public class Proceed : XmppElement
    {
        public Proceed(XElement other)
            : base(other)
        {
        }

        public Proceed()
            : base(ElementName)
        {
        }

        public static XName ElementName => XName.Get("proceed", Namespaces.Tls);
    }
}
