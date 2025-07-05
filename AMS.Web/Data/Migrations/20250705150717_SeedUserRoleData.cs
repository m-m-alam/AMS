using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMS.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserRoleData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var hasher = new PasswordHasher<IdentityUser>();
            var hashPassword = hasher.HashPassword(null, "123456");

            // Seed Role 
            migrationBuilder.Sql("INSERT INTO AspNetRoles (Id,Name,NormalizedName) VALUES ('" + Guid.NewGuid() + "','Admin ','ADMIN')");
            migrationBuilder.Sql("INSERT INTO AspNetRoles (Id,Name,NormalizedName) VALUES ('" + Guid.NewGuid() + "','Accountant','ACCOUNTANT')");
            migrationBuilder.Sql("INSERT INTO AspNetRoles (Id,Name,NormalizedName) VALUES ('" + Guid.NewGuid() + "','Viewer','VIEWER')");

            // Seed User
            migrationBuilder.Sql("INSERT INTO AspNetUsers (Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,AccessFailedCount,SecurityStamp)" +
                " VALUES ('" + Guid.NewGuid() + "','admin@gmail.com','ADMIN@GMAIL.COM','admin@gmail.com','ADMIN@GMAIL.COM','true','" + hashPassword + "','false','false','false','0','" + Guid.NewGuid() + "')");
            
             migrationBuilder.Sql("INSERT INTO AspNetUsers (Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnabled,AccessFailedCount,SecurityStamp)" +
                " VALUES ('" + Guid.NewGuid() + "','Viewer@gmail.com','VIEWER@GMAIL.COM','Viewer@gmail.com','VIEWER@GMAIL.COM','true','" + hashPassword + "','false','false','false','0','" + Guid.NewGuid() + "')");
            

            // Seed UserRole
            migrationBuilder.Sql("INSERT INTO AspNetUserRoles (UserId,RoleId) VALUES ((SELECT Id FROM AspNetUsers WHERE UserName = 'admin@gmail.com')," +
                "(SELECT Id FROM AspNetRoles WHERE Name = 'Admin'))");
            migrationBuilder.Sql("INSERT INTO AspNetUserRoles (UserId,RoleId) VALUES ((SELECT Id FROM AspNetUsers WHERE UserName = 'Viewer@gmail.com')," +
                "(SELECT Id FROM AspNetRoles WHERE Name = 'Viewer'))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM AspNetUsers WHERE UserName IN ('admin@gmail.com','Viewer@gmail.com')");
            migrationBuilder.Sql("DELETE FROM AspNetRoles WHERE Name IN ('Admin','Viewer')");
        }
    }
}
