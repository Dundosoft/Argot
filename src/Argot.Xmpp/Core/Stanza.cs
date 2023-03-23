using System.Xml.Linq;

namespace Argot.XMPP.Core;

public class Stanza : XElement
{
    public Stanza(XName name) : base(name)
    {
    }

    public string ID
    {
        get => Attribute("id")?.Value;
        set => SetAttributeValue("id", value);
    }

    public JID From => new(Attribute("from")?.Value);

    public JID To
    {
        get => new(Attribute("to")?.Value);
        set => SetAttributeValue("to", value.FullJid);
    }

    public static T Parse<T>(XElement src) where T : XElement, new()
    {
        if (src is null) return null;

        if (src is not T stanza)
        {
            stanza = new T();
            stanza.ReplaceAttributes(src.Attributes());
            stanza.ReplaceNodes(src.Nodes());
            if (!src.Name.Equals(stanza.Name))
                return null;
        }

        return stanza;
    }
}
