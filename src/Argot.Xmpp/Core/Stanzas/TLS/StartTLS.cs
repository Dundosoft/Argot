using System.Xml.Linq;

namespace Argot.XMPP.Core.Stanzas.TLS;

public class StartTLS : Stanza
{
    public StartTLS() : base(XNamespace.Get(Namespaces.XmppTls) + "starttls")
    {

    }
}
