using System.Xml.Linq;

namespace Xmpp.Core.Stanza
{
    public class Stanza : XmppElement
    {
        public Stanza(XName name) : base(name) 
        { }
        public Stanza(XElement other) : base(other)
        {
        }
    }
}
