using AMS.Web.Data;
using AMS.Web.Models;
using AMS.Web.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AMS.Web.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly string _connectionString;
        private readonly ApplicationDbContext _context;

        public VoucherService(IConfiguration configuration, ApplicationDbContext context)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        }

        public async Task<List<VoucherType>> GetVoucherTypesAsync()
        {
            return await _context.VoucherTypes.ToListAsync();
        }

        public async Task<int> SaveVoucherAsync(VoucherViewModel voucherViewModel, string userId)
        {
            // Create XML for voucher details
            var detailsXml = "<VoucherDetails>";
            foreach (var detail in voucherViewModel.Details)
            {
                detailsXml += $@"
                <Detail>
                    <AccountId>{detail.AccountId}</AccountId>
                    <DebitAmount>{detail.DebitAmount}</DebitAmount>
                    <CreditAmount>{detail.CreditAmount}</CreditAmount>
                    <Description>{detail.Description}</Description>
                </Detail>";
            }
            detailsXml += "</VoucherDetails>";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_SaveVoucher", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@VoucherNumber", voucherViewModel.VoucherNumber);
            command.Parameters.AddWithValue("@VoucherDate", voucherViewModel.VoucherDate);
            command.Parameters.AddWithValue("@VoucherTypeId", voucherViewModel.VoucherTypeId);
            command.Parameters.AddWithValue("@ReferenceNo", voucherViewModel.ReferenceNo ?? "");
            command.Parameters.AddWithValue("@Description", voucherViewModel.Description ?? "");
            command.Parameters.AddWithValue("@CreatedBy", userId);
            command.Parameters.AddWithValue("@VoucherDetailsXML", detailsXml);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<List<Voucher>> GetVouchersAsync()
        {
            var vouchers = new List<Voucher>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetVoucherDetails", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            var voucherDict = new Dictionary<int, Voucher>();

            while (await reader.ReadAsync())
            {
                var voucherId = reader.GetInt32("Id");

                if (!voucherDict.ContainsKey(voucherId))
                {
                    voucherDict[voucherId] = new Voucher
                    {
                        Id = voucherId,
                        VoucherNumber = reader.GetString("VoucherNumber"),
                        VoucherDate = reader.GetDateTime("VoucherDate"),
                        ReferenceNo = reader.IsDBNull("ReferenceNo") ? "" : reader.GetString("ReferenceNo"),
                        Description = reader.IsDBNull("Description") ? "" : reader.GetString("Description"),
                        TotalAmount = reader.GetDecimal("TotalAmount"),
                        VoucherType = new VoucherType { TypeName = reader.GetString("TypeName") },
                        CreatedDate = reader.GetDateTime("CreatedDate"),
                        IsPosted = reader.GetBoolean("IsPosted"),
                        VoucherDetails = new List<VoucherDetail>()
                    };
                }

                if (!reader.IsDBNull("DetailId"))
                {
                    voucherDict[voucherId].VoucherDetails.Add(new VoucherDetail
                    {
                        Id = reader.GetInt32("DetailId"),
                        AccountId = reader.GetInt32("AccountId"),
                        AccountName = reader.GetString("AccountName"),
                        AccountCode = reader.GetString("AccountCode"),
                        DebitAmount = reader.GetDecimal("DebitAmount"),
                        CreditAmount = reader.GetDecimal("CreditAmount"),
                        Description = reader.IsDBNull("DetailDescription") ? "" : reader.GetString("DetailDescription")
                    });
                }
            }

            return voucherDict.Values.ToList();
        }

        public async Task<Voucher> GetVoucherDetailsAsync(int voucherId)
        {
            var vouchers = await GetVouchersAsync();
            return vouchers.FirstOrDefault(v => v.Id == voucherId);
        }
    }
}