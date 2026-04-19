using Microsoft.EntityFrameworkCore;
using Gateway.BlindMatch.Models;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Gateway.BlindMatch.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<ProjectProposal> ProjectProposals { get; set; }
    public DbSet<ResearchArea> ResearchAreas { get; set; }
    public DbSet<SupervisorExpertise> SupervisorExpertises { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToCollection("users");
        
        // Ensure complex properties work correctly for MongoDB
        builder.Entity<SupervisorExpertise>()
            .HasKey(se => new { se.SupervisorId, se.ResearchAreaId });
    }
}
