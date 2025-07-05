using AMS.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AMS.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }
        public DbSet<ChartOfAccount> ChartOfAccounts { get; set; }
        public DbSet<VoucherType> VoucherTypes { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<VoucherDetail> VoucherDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure entities
            //builder.Entity<ChartOfAccount>()
            //    .HasKey(c => c.Id);

            //builder.Entity<Voucher>()
            //    .HasMany(v => v.VoucherDetails)
            //    .WithOne(vd => vd.Voucher)
            //    .HasForeignKey(vd => vd.VoucherId);
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
