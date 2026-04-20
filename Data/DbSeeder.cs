using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Gateway.BlindMatch.Models;

namespace Gateway.BlindMatch.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<MongoRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // ── Seed Roles ──────────────────────────────────────
            string[] roleNames = { "System Administrator", "Module Leader", "Supervisor", "Student" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new MongoRole(roleName));
                }
            }

            // ── Seed Admin ──────────────────────────────────────
            var adminEmail = "admin@blindmatch.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Default Admin",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "System Administrator");
            }

            // ── Seed Module Leader ──────────────────────────────
            var moduleLeaderEmail = "moduleleader@blindmatch.com";
            var mlUser = await userManager.FindByEmailAsync(moduleLeaderEmail);
            if (mlUser == null)
            {
                mlUser = new ApplicationUser
                {
                    UserName = moduleLeaderEmail,
                    Email = moduleLeaderEmail,
                    FullName = "Module Leader",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(mlUser, "ModuleLeader@123");
                await userManager.AddToRoleAsync(mlUser, "Module Leader");
            }

            // ── Seed Student ────────────────────────────────────
            var studentEmail = "student@blindmatch.com";
            var studentUser = await userManager.FindByEmailAsync(studentEmail);
            if (studentUser == null)
            {
                studentUser = new ApplicationUser
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    FullName = "Test Student",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(studentUser, "Student@123");
                await userManager.AddToRoleAsync(studentUser, "Student");
            }

            // ── Seed Supervisor ─────────────────────────────────
            var supervisorEmail = "supervisor@blindmatch.com";
            var supervisorUser = await userManager.FindByEmailAsync(supervisorEmail);
            if (supervisorUser == null)
            {
                supervisorUser = new ApplicationUser
                {
                    UserName = supervisorEmail,
                    Email = supervisorEmail,
                    FullName = "Test Supervisor",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(supervisorUser, "Supervisor@123");
                await userManager.AddToRoleAsync(supervisorUser, "Supervisor");
            }

            // ── Seed 15 Research Areas ──────────────────────────
            string[] researchAreaNames =
            {
                "Artificial Intelligence",
                "Machine Learning",
                "Cybersecurity",
                "Data Science & Analytics",
                "Cloud Computing",
                "Internet of Things (IoT)",
                "Natural Language Processing",
                "Computer Vision",
                "Blockchain Technology",
                "Software Engineering",
                "Robotics & Automation",
                "Human-Computer Interaction",
                "Bioinformatics",
                "Quantum Computing",
                "Network & Distributed Systems"
            };

            var existingNames = await context.ResearchAreas
                .Select(r => r.Name)
                .ToListAsync();

            var newNames = researchAreaNames
                .Where(name => !existingNames.Contains(name))
                .ToList();

            if (newNames.Count > 0)
            {
                // MongoDB doesn't auto-generate int IDs, so compute the next ID manually
                var maxId = await context.ResearchAreas.AnyAsync()
                    ? await context.ResearchAreas.MaxAsync(r => r.Id)
                    : 0;

                var toAdd = new List<ResearchArea>();
                foreach (var name in newNames)
                {
                    maxId++;
                    toAdd.Add(new ResearchArea { Id = maxId, Name = name });
                }

                context.ResearchAreas.AddRange(toAdd);
                await context.SaveChangesAsync();
            }
        }
    }
}
