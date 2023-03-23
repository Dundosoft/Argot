using System.Text.RegularExpressions;

namespace Argot.XMPP.Core;

public partial class JID
{
    [GeneratedRegex(@"(?'local'.+?)@(?'domain'.+?)(/(?'resource'.+?))?")]
    private static partial Regex JIDRegex();
    public string Local { get; set; }
    public string Domain { get; set; }
    public string Resource { get; set; }

    public JID()
    {
    }

    public JID(string jid)
    {
        if (string.IsNullOrEmpty(jid))
            return;

        var regexMatch = JIDRegex().Match(jid);

        if (regexMatch.Success)
        {
            Local = regexMatch.Groups.GetValueOrDefault("local")?.Value;
            Domain = regexMatch.Groups.GetValueOrDefault("domain")?.Value;
            Resource = regexMatch.Groups.GetValueOrDefault("resource")?.Value;
        }
    }

    public string BareJid =>
        string.IsNullOrEmpty(Local) ? Domain : $"{Local}@{Domain}";

    public string FullJid =>
        string.IsNullOrEmpty(Resource) ? BareJid : $"{BareJid}/{Resource}";

    public override string ToString() => FullJid;
}
