namespace Argot.Common.Settings;

public interface ISettingsService
{
    public Account GetAccount(Guid name);
    public void SaveAccount(Account account);
}