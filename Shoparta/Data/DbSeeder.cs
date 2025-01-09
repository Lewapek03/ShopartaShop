using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Shoparta.Constans;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shoparta.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefault(IServiceProvider service)
        {
            var logger = service.GetService<ILogger<DbSeeder>>();
            var userMgr = service.GetService<UserManager<IdentityUser>>();
            var roleMgr = service.GetService<RoleManager<IdentityRole>>();

            if (userMgr == null || roleMgr == null)
            {
                throw new ArgumentNullException("UserManager or RoleManager is not available.");
            }

            try
            {
                foreach (var role in Enum.GetValues(typeof(Roles)))
                {
                    var roleName = role.ToString();
                    if (!await roleMgr.RoleExistsAsync(roleName))
                    {
                        var result = await roleMgr.CreateAsync(new IdentityRole(roleName));
                        if (result.Succeeded)
                        {
                            logger?.LogInformation($"Created role: {roleName}");
                        }
                        else
                        {
                            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                            logger?.LogError($"Failed to create role {roleName}: {errors}");
                            throw new Exception($"Failed to create role {roleName}: {errors}");
                        }
                    }
                }

                var adminEmail = "admin@shoparta.com";
                var adminPassword = "Admin123!";

                var adminUser = await userMgr.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var createAdminResult = await userMgr.CreateAsync(adminUser, adminPassword);
                    if (createAdminResult.Succeeded)
                    {
                        var addToRoleResult = await userMgr.AddToRoleAsync(adminUser, Roles.Admin.ToString());
                        if (addToRoleResult.Succeeded)
                        {
                            logger?.LogInformation($"Created admin user: {adminEmail} and assigned role {Roles.Admin}");
                        }
                        else
                        {
                            var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
                            logger?.LogError($"Failed to assign role {Roles.Admin} to admin user: {errors}");
                            throw new Exception($"Failed to assign role {Roles.Admin} to admin user: {errors}");
                        }
                    }
                    else
                    {
                        var errors = string.Join(", ", createAdminResult.Errors.Select(e => e.Description));
                        logger?.LogError($"Failed to create admin user: {errors}");
                        throw new Exception($"Failed to create admin user: {errors}");
                    }
                }
                else
                {
                    if (!await userMgr.IsInRoleAsync(adminUser, Roles.Admin.ToString()))
                    {
                        var addToRoleResult = await userMgr.AddToRoleAsync(adminUser, Roles.Admin.ToString());
                        if (addToRoleResult.Succeeded)
                        {
                            logger?.LogInformation($"Assigned role {Roles.Admin} to existing admin user: {adminEmail}");
                        }
                        else
                        {
                            var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
                            logger?.LogError($"Failed to assign role {Roles.Admin} to existing admin user: {errors}");
                            throw new Exception($"Failed to assign role {Roles.Admin} to existing admin user: {errors}");
                        }
                    }
                }

                var analystEmail = "analyst@shoparta.com";
                var analystPassword = "Analyst123!";

                var analystUser = await userMgr.FindByEmailAsync(analystEmail);
                if (analystUser == null)
                {
                    analystUser = new IdentityUser
                    {
                        UserName = analystEmail,
                        Email = analystEmail,
                        EmailConfirmed = true
                    };

                    var createAnalystResult = await userMgr.CreateAsync(analystUser, analystPassword);
                    if (createAnalystResult.Succeeded)
                    {
                        var addToRoleResult = await userMgr.AddToRoleAsync(analystUser, Roles.Analyst.ToString());
                        if (addToRoleResult.Succeeded)
                        {
                            logger?.LogInformation($"Created analyst user: {analystEmail} and assigned role {Roles.Analyst}");
                        }
                        else
                        {
                            var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
                            logger?.LogError($"Failed to assign role {Roles.Analyst} to analyst user: {errors}");
                            throw new Exception($"Failed to assign role {Roles.Analyst} to analyst user: {errors}");
                        }
                    }
                    else
                    {
                        var errors = string.Join(", ", createAnalystResult.Errors.Select(e => e.Description));
                        logger?.LogError($"Failed to create analyst user: {errors}");
                        throw new Exception($"Failed to create analyst user: {errors}");
                    }
                }
                else
                {
                    if (!await userMgr.IsInRoleAsync(analystUser, Roles.Analyst.ToString()))
                    {
                        var addToRoleResult = await userMgr.AddToRoleAsync(analystUser, Roles.Analyst.ToString());
                        if (addToRoleResult.Succeeded)
                        {
                            logger?.LogInformation($"Assigned role {Roles.Analyst} to existing analyst user: {analystEmail}");
                        }
                        else
                        {
                            var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
                            logger?.LogError($"Failed to assign role {Roles.Analyst} to existing analyst user: {errors}");
                            throw new Exception($"Failed to assign role {Roles.Analyst} to existing analyst user: {errors}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError($"An error occurred during database seeding: {ex.Message}");
                throw;
            }
        }
    }
}
