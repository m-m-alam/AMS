namespace AMS.Web.ViewModels
{
    public class VoucherDetailViewModel
    {
        public int AccountId { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Description { get; set; }
    }
}
