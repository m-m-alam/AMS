namespace AMS.Web.Models
{
    public class VoucherDetail
    {
        public int Id { get; set; }
        public int VoucherId { get; set; }
        public int AccountId { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Description { get; set; }

        // Navigation properties
        public Voucher Voucher { get; set; }
        public ChartOfAccount Account { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
    }
}
