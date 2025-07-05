using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AMS.Web.Services
{
    public class RoleStore : IRoleStore<IdentityRole>
    {
        private readonly string _connectionString;
        private readonly ILogger<UserStore> _logger;

        public RoleStore(IConfiguration configuration, ILogger<UserStore> logger) 
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

      
        private IdentityRole MapToApplicationRole(SqlDataReader reader)
        {
            if (!reader.HasRows && !reader.IsClosed) return null;

            return new IdentityRole
            {
                Id = reader["Id"]?.ToString(),
                Name = reader["Name"] as string,
                NormalizedName = reader["NormalizedName"] as string,
                ConcurrencyStamp = reader["ConcurrencyStamp"] as string
            };
        }


        public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));

            role.Id = role.Id ?? Guid.NewGuid().ToString();
            role.ConcurrencyStamp = Guid.NewGuid().ToString();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (SqlCommand command = new SqlCommand("sp_CreateRole", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", role.Id);
                        command.Parameters.AddWithValue("@Name", (object)role.Name ?? DBNull.Value);
                        command.Parameters.AddWithValue("@NormalizedName", (object)role.NormalizedName ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ConcurrencyStamp", (object)role.ConcurrencyStamp ?? DBNull.Value);
                        _logger.LogInformation("Attempting to create role: {RoleName}", role.Name);
                        await command.ExecuteNonQueryAsync(cancellationToken);
                        _logger.LogInformation("Role {RoleName} created successfully.", role.Name);
                    }
                }
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role {RoleName}: {Message}", role.Name, ex.Message);
                return IdentityResult.Failed(new IdentityError { Description = $"Failed to create role: {ex.Message}" });
            }
        }

        public async Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));

            role.ConcurrencyStamp = Guid.NewGuid().ToString();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (SqlCommand command = new SqlCommand("sp_UpdateRole", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", role.Id);
                        command.Parameters.AddWithValue("@Name", (object)role.Name ?? DBNull.Value);
                        command.Parameters.AddWithValue("@NormalizedName", (object)role.NormalizedName ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ConcurrencyStamp", (object)role.ConcurrencyStamp ?? DBNull.Value);
                        _logger.LogInformation("Attempting to update role: {RoleName}", role.Name);
                        await command.ExecuteNonQueryAsync(cancellationToken);
                        _logger.LogInformation("Role {RoleName} updated successfully.", role.Name);
                    }
                }
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {RoleName}: {Message}", role.Name, ex.Message);
                return IdentityResult.Failed(new IdentityError { Description = $"Failed to update role: {ex.Message}" });
            }
        }

        public async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (SqlCommand command = new SqlCommand("sp_DeleteRole", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", role.Id);
                        _logger.LogInformation("Attempting to delete role: {RoleId}", role.Id);
                        await command.ExecuteNonQueryAsync(cancellationToken);
                        _logger.LogInformation("Role {RoleId} deleted successfully.", role.Id);
                    }
                }
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {RoleId}: {Message}", role.Id, ex.Message);
                return IdentityResult.Failed(new IdentityError { Description = $"Failed to delete role: {ex.Message}" });
            }
        }

        public async Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (SqlCommand command = new SqlCommand("sp_FindRoleById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", roleId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                        {
                            if (await reader.ReadAsync(cancellationToken))
                            {
                                return MapToApplicationRole(reader);
                            }
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding role by ID {RoleId}: {Message}", roleId, ex.Message);
                throw; 
            }
        }

        public async Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (SqlCommand command = new SqlCommand("sp_FindRoleByName", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@NormalizedName", normalizedRoleName);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                        {
                            if (await reader.ReadAsync(cancellationToken))
                            {
                                return MapToApplicationRole(reader);
                            }
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding role by name {NormalizedRoleName}: {Message}", normalizedRoleName, ex.Message);
                throw; 
            }
        }

        public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        // NEW: Method to get all roles directly from the store
        public async Task<List<IdentityRole>> GetAllRolesAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            List<IdentityRole> roles = new List<IdentityRole>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (SqlCommand command = new SqlCommand("sp_GetAllRoles", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                roles.Add(MapToApplicationRole(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles: {Message}", ex.Message);
                throw;
            }
            return roles;
        }

        public void Dispose()
        {
            
        }
    }
}

