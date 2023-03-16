namespace Xmpp.Core
{
    // TODO: validation and a bunch of other crap
    public class Jid
    {
        private readonly string _resource;
        private readonly string _local;
        private readonly string _hostname;

        public Jid(string jid)
        {
            (_local, _hostname, _resource) = Parse(jid);
        }

        public string Local => _local;
        public string Hostname => _hostname;
        public string Resource => _resource;

        public static (string local, string host, string resource) Parse(string jid)
        {
            var atIndex = jid.IndexOf('@', StringComparison.Ordinal);
            var slashIndex = jid.IndexOf('/', StringComparison.Ordinal);
            string local = null;
            string host = null;
            string resource = null;

            if (atIndex == -1)
            {
                return (local, host, resource);
            }

            local = jid[..atIndex];

            if (slashIndex == -1)
            {
                // TODO: generate resource id
                return (local, jid[(atIndex + 1)..], "ARGOT");
            }

            host = jid.Substring(atIndex + 1, slashIndex);
            resource = jid[(slashIndex + 1)..];

            return (local, host, resource);
        }
    }
}
