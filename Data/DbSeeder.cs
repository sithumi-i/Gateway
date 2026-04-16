using Microsoft.AspNetCore.Identity;

namespace Gateway.BlindMatch.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Models.ApplicationUser>>();

            string[] roleNames = { "System Administrator", "Module Leader", "Supervisor", "Student" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminEmail = "admin@blindmatch.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new Models.ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Default Admin",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "System Administrator");
            }
            
            var moduleLeaderEmail = "moduleleader@blindmatch.com";
            var mlUser = await userManager.FindByEmailAsync(moduleLeaderEmail);
            if (mlUser == null)
            {
                mlUser = new Models.ApplicationUser
                {
                    UserName = moduleLeaderEmail,
                    Email = moduleLeaderEmail,
                    FullName = "Module Leader",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(mlUser, "ModuleLeader@123");
                await userManager.AddToRoleAsync(mlUser, "Module Leader");
            }
            
            var studentEmail = "student@blindmatch.com";
            var studentUser = await userManager.FindByEmailAsync(studentEmail);
            if (studentUser == null)
            {
                studentUser = new Models.ApplicationUser
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    FullName = "Test Student",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(studentUser, "Student@123");
                await userManager.AddToRoleAsync(studentUser, "Student");
            }

            var supervisorEmail = "supervisor@blindmatch.com";
            var supervisorUser = await userManager.FindByEmailAsync(supervisorEmail);
            if (supervisorUser == null)
            {
                supervisorUser = new Models.ApplicationUser
                {
                    UserName = supervisorEmail,
                    Email = supervisorEmail,
                    FullName = "Test Supervisor",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(supervisorUser, "Supervisor@123");
                await userManager.AddToRoleAsync(supervisorUser, "Supervisor");
            }
        }
    }
}
