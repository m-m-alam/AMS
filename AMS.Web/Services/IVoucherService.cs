using AMS.Web.Models;
using AMS.Web.ViewModels;

namespace AMS.Web.Services
{
    public interface IVoucherService
    {
        Task<List<Voucher>> GetVouchersAsync();
        Task<List<VoucherType>> GetVoucherTypesAsync();
        Task<int> SaveVoucherAsync(VoucherViewModel voucherViewModel, string userId);
        Task<Voucher> GetVoucherDetailsAsync(int voucherId);
    }
}
