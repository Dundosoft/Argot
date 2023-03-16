using System.Xml.Linq;

namespace Xmpp.Core.Stanza.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class XmppElementAttribute : Attribute
    {
        public XmppElementAttribute(string name, string xmlNamespace, Type type)
        {
            Name = XName.Get(name, xmlNamespace);
            Type = type;
        }

        public XName Name { get; }
        public Type Type { get; }
    }
}
