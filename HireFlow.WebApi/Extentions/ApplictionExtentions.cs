using HireFlow.Business.Authentications.Constants;
using HireFlow.Domain.Entities;
using HireFlow.WebApi.Configurtions;
using Microsoft.AspNetCore.Identity;

namespace HireFlow.WebApi.Extentions;

public static class ApplicationExtensions
{
    
    public static async Task SeedDataBaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        
       
        await SeedRolesAsync(scope.ServiceProvider);
        
        await SeedAdminsAsync(scope.ServiceProvider);
    }

    
    private static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        
        
        if (roleManager.Roles.Any()) return;

        
        var adminRole = new Role(RoleConstants.AdminRoleName);
        var employerRole = new Role(RoleConstants.EmployerRoleName);
        var jobSeekerRole = new Role(RoleConstants.JobSeekerRoleName);

       
        await roleManager.CreateAsync(adminRole);
        await roleManager.CreateAsync(employerRole);
        await roleManager.CreateAsync(jobSeekerRole);
    }
    private static async Task SeedAdminsAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        
        var adminData = configuration.GetSection("AdminData").Get<AdminData>();

        if (adminData == null) return;

   
        var existingAdmin = await userManager.FindByNameAsync(adminData.Username);

        if (existingAdmin == null)
        {
            
            var adminUser = new User(adminData.Username, isApproved: true)
            {
                Email = "admin@hireflow.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };

            
            adminUser.UpdateProfile(adminData.FirstName, adminData.LastName, adminUser.Id);

            
            var result = await userManager.CreateAsync(adminUser, adminData.Password);

            if (result.Succeeded)
            { 
                await userManager.AddToRoleAsync(adminUser, RoleConstants.AdminRoleName);
            }
            else
            {
                
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to seed admin user. Errors: {errors}");
            }
        }
    }
}