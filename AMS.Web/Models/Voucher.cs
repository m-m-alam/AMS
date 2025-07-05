namespace AMS.Web.Models
{
    public class Voucher
    {
        public int Id { get; set; }
        public string VoucherNumber { get; set; }
        public DateTime VoucherDate { get; set; }
        public int VoucherTypeId { get; set; }
        public string ReferenceNo { get; set; }
        public string Description { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool? IsPosted { get; set; }

        public VoucherType VoucherType { get; set; }
        public List<VoucherDetail> VoucherDetails { get; set; }
    }
}
