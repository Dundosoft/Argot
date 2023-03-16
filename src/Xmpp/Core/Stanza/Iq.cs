using System.Xml.Linq;
using Xmpp.Core.Stanza.Attributes;

namespace Xmpp.Core.Stanza
{
    public enum IqType
    {
        Error,
        Get,
        Set,
        Result
    }

    [XmppElement("iq", Namespaces.Client, typeof(Iq))]
    public class Iq : Stanza
    {
        public Iq() : base(ElementName)
        {
        }

        public static XName ElementName => XName.Get("iq", Namespaces.Client);

        public IqType Type
        {
            get => GetAttributeEnumValue<IqType>("type");
            set => SetAttributeEnumValue("type", value);
        }
    }
}
