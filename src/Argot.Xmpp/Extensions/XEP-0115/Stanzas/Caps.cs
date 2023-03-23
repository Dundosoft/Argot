using System.Xml.Linq;

namespace Argot.XMPP.Core.Stanzas;

public class Caps : Stanza
{
    public Caps(string node, string hash)
        : base(XNamespace.Get(Namespaces.EntityCaps) + "c")
    {
        SetAttributeValue("hash", "sha-1");
        SetAttributeValue("node", node);
        SetAttributeValue("ver", hash);
    }
}
