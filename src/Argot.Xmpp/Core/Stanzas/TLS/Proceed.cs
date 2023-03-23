using System.Xml.Linq;

namespace Argot.XMPP.Core.Stanzas.TLS;

public class Proceed : Stanza
{
    public Proceed()
        : base(XNamespace.Get(Namespaces.XmppTls) + "proceed")
    {

    }
}
