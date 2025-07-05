namespace AMS.Web.Models
{
    public class ChartOfAccount
    {
        public int Id { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public int? ParentAccountId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        // Navigation properties
        public ChartOfAccount? ParentAccount { get; set; }
        public List<ChartOfAccount> ChildAccounts { get; set; }
        public string ParentAccountName { get; set; }
    }
}
