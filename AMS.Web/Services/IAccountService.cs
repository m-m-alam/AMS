using AMS.Web.Models;

namespace AMS.Web.Services
{
    public interface IAccountService
    {
        Task<List<ChartOfAccount>> GetAccountsAsync();
        Task<ChartOfAccount> GetAccountByIdAsync(int id);
        Task<int> CreateAccountAsync(ChartOfAccount account);
        Task<bool> UpdateAccountAsync(ChartOfAccount account);
        Task<bool> DeleteAccountAsync(int id);
    }
}
