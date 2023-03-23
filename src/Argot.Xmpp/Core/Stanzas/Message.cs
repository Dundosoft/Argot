using System.Xml.Linq;

namespace Argot.XMPP.Core.Stanzas;

public partial class Message : Stanza
{
    public Message()
        : base(XNamespace.Get(Namespaces.JabberClient) + "message")
    {

    }

    public string Text
    {
        get
        {
            return Element(XNamespace.Get(Namespaces.JabberClient) + "body") == null ? string.Empty : Element(XNamespace.Get(Namespaces.JabberClient) + "body").Value;
        }
        set
        {
            var body = Element(XNamespace.Get(Namespaces.JabberClient) + "body") ??
                       new XElement(XNamespace.Get(Namespaces.JabberClient) + "body");
            body.SetValue(value);
            Add(body);
        }
    }
}
