using System;
namespace Argot.Common.Settings
{
    public class Account
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string JID { get; set; }
        public string Password { get; set; }
        public string Hostname { get; set; }
        public string Port { get; set; }
        public bool PreferTLS { get; set; }
        public string Language { get; set; }
    }
}
