using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Argot.Common.Settings
{
    public class SettingsService : ISettingsService
    {
        private readonly IPreferences preferences;
        private readonly ISecureStorage secureStorage;

        public SettingsService(IPreferences preferences, ISecureStorage secureStorage)
        {
            this.preferences = preferences;
            this.secureStorage = secureStorage;
        }

        public Account GetAccount(Guid accountName) =>
            new() { JID = "steve@bonerbonerboner.com", Password = "Jbcbd99c!" };
        public void SaveAccount(Account account) { }
    }
}
