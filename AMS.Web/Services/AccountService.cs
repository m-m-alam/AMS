using AMS.Web.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AMS.Web.Services
{
    public class AccountService : IAccountService
    {
        private readonly string _connectionString;

        public AccountService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<ChartOfAccount>> GetAccountsAsync()
        {
            var accounts = new List<ChartOfAccount>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ManageChartOfAccounts", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "SELECT");

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
                while (await reader.ReadAsync())
                {
                    accounts.Add(new ChartOfAccount
                    {
                        Id = reader.GetInt32("Id"),
                        AccountCode = reader.GetString("AccountCode"),
                        AccountName = reader.GetString("AccountName"),
                        AccountType = reader.GetString("AccountType"),
                        ParentAccountId = reader.IsDBNull("ParentAccountId") ? null : reader.GetInt32("ParentAccountId"),
                        IsActive = reader.GetBoolean("IsActive"),
                        CreatedDate = reader.IsDBNull("CreatedDate") ? null : reader.GetDateTime("CreatedDate"),
                        ParentAccountName = reader.IsDBNull("ParentAccountName") ? null : reader.GetString("ParentAccountName")
                    });
                }
                        

            return accounts;
        }

        public async Task<ChartOfAccount> GetAccountByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ManageChartOfAccounts", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "SELECT");
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new ChartOfAccount
                {
                    Id = reader.GetInt32("Id"),
                    AccountCode = reader.GetString("AccountCode"),
                    AccountName = reader.GetString("AccountName"),
                    AccountType = reader.GetString("AccountType"),
                    ParentAccountId = reader.IsDBNull("ParentAccountId") ? null : reader.GetInt32("ParentAccountId"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedDate = reader.GetDateTime("CreatedDate")
                };
            }

            return null;
        }

        public async Task<int> CreateAccountAsync(ChartOfAccount account)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ManageChartOfAccounts", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "INSERT");
            command.Parameters.AddWithValue("@AccountCode", account.AccountCode);
            command.Parameters.AddWithValue("@AccountName", account.AccountName);
            command.Parameters.AddWithValue("@AccountType", account.AccountType);
            command.Parameters.AddWithValue("@ParentAccountId", (object)account.ParentAccountId ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", account.IsActive);
            command.Parameters.AddWithValue("@CreatedBy", account.CreatedBy ?? "System");

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAccountAsync(ChartOfAccount account)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ManageChartOfAccounts", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "UPDATE");
            command.Parameters.AddWithValue("@Id", account.Id);
            command.Parameters.AddWithValue("@AccountCode", account.AccountCode);
            command.Parameters.AddWithValue("@AccountName", account.AccountName);
            command.Parameters.AddWithValue("@AccountType", account.AccountType);
            command.Parameters.AddWithValue("@ParentAccountId", (object)account.ParentAccountId ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", account.IsActive);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAccountAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ManageChartOfAccounts", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "DELETE");
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}