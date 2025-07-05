using AMS.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;

namespace AMS.Web.Services
{
    public class UserStore : IUserStore<IdentityUser>,
        IUserPasswordStore<IdentityUser>,
        IUserRoleStore<IdentityUser>,
        IUserEmailStore<IdentityUser>
    {

        private readonly string _connectionString;
        private readonly ILogger<UserStore> _logger;

        public UserStore(IConfiguration configuration, ILogger<UserStore> logger) 
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        private IdentityUser MapToApplicationUser(SqlDataReader reader)
        {
            
            if (!reader.HasRows && !reader.IsClosed) return null;
          
            return new IdentityUser
            {
                Id = reader["Id"]?.ToString(),
                UserName = reader["UserName"] as string,
                NormalizedUserName = reader["NormalizedUserName"] as string,
                Email = reader["Email"] as string,
                NormalizedEmail = reader["NormalizedEmail"] as string,
                EmailConfirmed = reader["EmailConfirmed"] != DBNull.Value ? (bool)reader["EmailConfirmed"] : false,
                PasswordHash = reader["PasswordHash"] as string,
                SecurityStamp = reader["SecurityStamp"] as string,
                ConcurrencyStamp = reader["ConcurrencyStamp"] as string,
                PhoneNumber = reader["PhoneNumber"] as string,
                PhoneNumberConfirmed = reader["PhoneNumberConfirmed"] != DBNull.Value ? (bool)reader["PhoneNumberConfirmed"] : false,
                TwoFactorEnabled = reader["TwoFactorEnabled"] != DBNull.Value ? (bool)reader["TwoFactorEnabled"] : false,
                LockoutEnd = reader["LockoutEnd"] != DBNull.Value ? (DateTimeOffset?)reader["LockoutEnd"] : null,
                LockoutEnabled = reader["LockoutEnabled"] != DBNull.Value ? (bool)reader["LockoutEnabled"] : false,
                AccessFailedCount = reader["AccessFailedCount"] != DBNull.Value ? (int)reader["AccessFailedCount"] : 0
            };
        }

    
        public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.Id = user.Id ?? Guid.NewGuid().ToString();
            user.ConcurrencyStamp = Guid.NewGuid().ToString(); 

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (SqlCommand command = new SqlCommand("sp_CreateUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Id", user.Id);
                        command.Parameters.AddWithValue("@UserName", (object)user.UserName ?? DBNull.Value);
                        command.Parameters.AddWithValue("@NormalizedUserName", (object)user.NormalizedUserName ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value);
                        command.Parameters.AddWithValue("@NormalizedEmail", (object)user.NormalizedEmail ?? DBNull.Value);
                        command.Parameters.AddWithValue("@EmailConfirmed", user.EmailConfirmed);
                        command.Parameters.AddWithValue("@PasswordHash", (object)user.PasswordHash ?? DBNull.Value);
                        command.Parameters.AddWithValue("@SecurityStamp", (object)user.SecurityStamp ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ConcurrencyStamp", (object)user.ConcurrencyStamp ?? DBNull.Value);
                        command.Parameters.AddWithValue("@PhoneNumber", (object)user.PhoneNumber ?? DBNull.Value);
                        command.Parameters.AddWithValue("@PhoneNumberConfirmed", user.PhoneNumberConfirmed);
                        command.Parameters.AddWithValue("@TwoFactorEnabled", user.TwoFactorEnabled);
                        command.Parameters.AddWithValue("@LockoutEnd", (object)user.LockoutEnd ?? DBNull.Value);
                        command.Parameters.AddWithValue("@LockoutEnabled", user.LockoutEnabled);
                        command.Parameters.AddWithValue("@AccessFailedCount", user.AccessFailedCount);

                        _logger.LogInformation("Attempting to create user: {UserName}", user.UserName);
                        await command.ExecuteNonQueryAsync(cancellationToken);
                        _logger.LogInformation("User {UserName} created successfully.", user.UserName);
                    }
                }
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {UserName}: {Message}", user.UserName, ex.Message);
                return IdentityResult.Failed(new IdentityError { Description = $"Failed to create user: {ex.Message}" });
            }
        }

        public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.ConcurrencyStamp = Guid.NewGuid().ToString(); 

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (SqlCommand command = new SqlCommand("sp_UpdateUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Id", user.Id);
                        command.Parameters.AddWithValue("@UserName", (object)user.UserName ?? DBNull.Value);
                        command.Parameters.AddWithValue("@NormalizedUserName", (object)user.NormalizedUserName ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value);
                        command.Parameters.AddWithValue("@NormalizedEmail", (object)user.NormalizedEmail ?? DBNull.Value);
                        command.Parameters.AddWithValue("@EmailConfirmed", user.EmailConfirmed);
                        command.Parameters.AddWithValue("@PasswordHash", (object)user.PasswordHash ?? DBNull.Value);
                        command.Parameters.AddWithValue("@SecurityStamp", (object)user.SecurityStamp ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ConcurrencyStamp", (object)user.ConcurrencyStamp ?? DBNull.Value);
                        command.Parameters.AddWithValue("@PhoneNumber", (object)user.PhoneNumber ?? DBNull.Value);
                        command.Parameters.AddWithValue("@PhoneNumberConfirmed", user.PhoneNumberConfirmed);
                        command.Parameters.AddWithValue("@TwoFactorEnabled", user.TwoFactorEnabled);
                        command.Parameters.AddWithValue("@LockoutEnd", (object)user.LockoutEnd ?? DBNull.Value);
                        command.Parameters.AddWithValue("@LockoutEnabled", user.LockoutEnabled);
                        command.Parameters.AddWithValue("@AccessFailedCount", user.AccessFailedCount);

                        _logger.LogInformation("Attempting to update user: {UserName}", user.UserName);
                        await command.ExecuteNonQueryAsync(cancellationToken);
                        _logger.LogInformation("User {UserName} updated successfully.", user.UserName);
                    }
                }
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserName}: {Message}", user.UserName, ex.Message);
                return IdentityResult.Failed(new IdentityError { Description = $"Failed to update user: {ex.Message}" });
            }
        }

        public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (SqlCommand command = new SqlCommand("sp_DeleteUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", user.Id);
                        _logger.LogInformation("Attempting to delete user: {UserId}", user.Id);
                        await command.ExecuteNonQueryAsync(cancellationToken);
                        _logger.LogInformation("User {UserId} deleted successfully.", user.Id);
                    }
                }
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}: {Message}", user.Id, ex.Message);
                return IdentityResult.Failed(new IdentityError { Description = $"Failed to delete user: {ex.Message}" });
            }
        }

        public async Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (SqlCommand command = new SqlCommand("sp_FindUserById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", userId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                        {
                            if (await reader.ReadAsync(cancellationToken))
                            {
                                return MapToApplicationUser(reader);
                            }
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding user by ID {UserId}: {Message}", userId, ex.Message);
                throw; 
            }
        }

        public async Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (SqlCommand command = new SqlCommand("sp_FindUserByName", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@NormalizedUserName", normalizedUserName);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                        {
                            if (await reader.ReadAsync(cancellationToken))
                            {
                                return MapToApplicationUser(reader);
                            }
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding user by name {NormalizedUserName}: {Message}", normalizedUserName, ex.Message);
                throw; 
            }
        }

        public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            user.UserName = userName;
            user.NormalizedUserName = userName?.ToUpperInvariant();
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(IdentityUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }


        public async Task SetPasswordHashAsync(IdentityUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.PasswordHash = passwordHash;
         
            return;
        }

        public Task<string> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PasswordHash != null);
        }

  
        public Task SetEmailAsync(IdentityUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public async Task<IdentityUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (SqlCommand command = new SqlCommand("sp_FindUserByEmail", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@NormalizedEmail", normalizedEmail);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                        {
                            if (await reader.ReadAsync(cancellationToken))
                            {
                                return MapToApplicationUser(reader);
                            }
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding user by email {NormalizedEmail}: {Message}", normalizedEmail, ex.Message);
                throw; 
            }
        }

        public Task<string> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(IdentityUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }


        public async Task AddToRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(roleName));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    string roleId = null;
                    using (SqlCommand findRoleCommand = new SqlCommand("sp_FindRoleByName", connection))
                    {
                        findRoleCommand.CommandType = CommandType.StoredProcedure;
                        findRoleCommand.Parameters.AddWithValue("@NormalizedName", roleName.ToUpperInvariant());
                        using (SqlDataReader reader = await findRoleCommand.ExecuteReaderAsync(cancellationToken))
                        {
                            if (await reader.ReadAsync(cancellationToken))
                            {
                                roleId = reader["Id"]?.ToString();
                            }
                        }
                    }

                    if (roleId != null)
                    {
                        using (SqlCommand addRoleCommand = new SqlCommand("sp_AddUserToRole", connection))
                        {
                            addRoleCommand.CommandType = CommandType.StoredProcedure;
                            addRoleCommand.Parameters.AddWithValue("@UserId", user.Id);
                            addRoleCommand.Parameters.AddWithValue("@RoleId", roleId);
                            _logger.LogInformation("Attempting to add user {UserName} to role {RoleName}.", user.UserName, roleName);
                            await addRoleCommand.ExecuteNonQueryAsync(cancellationToken);
                            _logger.LogInformation("User {UserName} added to role {RoleName} successfully.", user.UserName, roleName);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Role '{RoleName}' not found when adding user '{UserName}' to role.", roleName, user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user {UserName} to role {RoleName}: {Message}", user.UserName, roleName, ex.Message);
                throw; 
            }
        }

        public async Task RemoveFromRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(roleName));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    string roleId = null;
                    using (SqlCommand findRoleCommand = new SqlCommand("sp_FindRoleByName", connection))
                    {
                        findRoleCommand.CommandType = CommandType.StoredProcedure;
                        findRoleCommand.Parameters.AddWithValue("@NormalizedName", roleName.ToUpperInvariant());
                        using (SqlDataReader reader = await findRoleCommand.ExecuteReaderAsync(cancellationToken))
                        {
                            if (await reader.ReadAsync(cancellationToken))
                            {
                                roleId = reader["Id"]?.ToString();
                            }
                        }
                    }

                    if (roleId != null)
                    {
                        using (SqlCommand removeRoleCommand = new SqlCommand("sp_RemoveUserFromRole", connection))
                        {
                            removeRoleCommand.CommandType = CommandType.StoredProcedure;
                            removeRoleCommand.Parameters.AddWithValue("@UserId", user.Id);
                            removeRoleCommand.Parameters.AddWithValue("@RoleId", roleId);
                            _logger.LogInformation("Attempting to remove user {UserName} from role {RoleName}.", user.UserName, roleName);
                            await removeRoleCommand.ExecuteNonQueryAsync(cancellationToken);
                            _logger.LogInformation("User {UserName} removed from role {RoleName} successfully.", user.UserName, roleName);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Role '{RoleName}' not found when removing user '{UserName}' from role.", roleName, user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user {UserName} from role {RoleName}: {Message}", user.UserName, roleName, ex.Message);
                throw; 
            }
        }

        public async Task<IList<string>> GetRolesAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));

            List<string> roles = new List<string>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (SqlCommand command = new SqlCommand("sp_GetUserRoles", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId", user.Id);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                roles.Add(reader["Name"]?.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles for user {UserName}: {Message}", user.UserName, ex.Message);
                throw; 
            }
            return roles;
        }

        public async Task<bool> IsInRoleAsync(IdentityUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(roleName));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (SqlCommand command = new SqlCommand("sp_IsUserInRole", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserId", user.Id);
                        command.Parameters.AddWithValue("@RoleName", roleName);

                        object result = await command.ExecuteScalarAsync(cancellationToken);
                        return (int)result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user {UserName} is in role {RoleName}: {Message}", user.UserName, roleName, ex.Message);
                throw; 
            }
        }

        public async Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(roleName));

           
            throw new NotImplementedException("GetUsersInRoleAsync is not implemented. Requires a new stored procedure 'sp_GetUsersInRole' and manual mapping.");
        }

    
        public async Task<List<IdentityUser>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            List<IdentityUser> users = new List<IdentityUser>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (SqlCommand command = new SqlCommand("sp_GetAllUsers", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                users.Add(MapToApplicationUser(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users: {Message}", ex.Message);
                throw;
            }
            return users;
        }


        public void Dispose()
        {
            
        }
    }
}

