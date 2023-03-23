using System.Xml.Linq;

namespace Argot.XMPP.Core.Stanzas;

public partial class Presence : Stanza
{
    public Presence()
       : base(XNamespace.Get(Namespaces.JabberClient) + "presence")
    {

    }
}
