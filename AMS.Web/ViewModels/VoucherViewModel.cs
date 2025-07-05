using AMS.Web.Models;

namespace AMS.Web.ViewModels
{
    public class VoucherViewModel
    {
        public int Id { get; set; }
        public string VoucherNumber { get; set; }
        public DateTime VoucherDate { get; set; }
        public int VoucherTypeId { get; set; }
        public string ReferenceNo { get; set; }
        public string Description { get; set; }
        public List<VoucherDetailViewModel> Details { get; set; } = new();
        public List<VoucherType> VoucherTypes { get; set; } = new();
        public List<ChartOfAccount> Accounts { get; set; } = new();
    }
}
