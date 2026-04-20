using Microsoft.EntityFrameworkCore;
using Gateway.BlindMatch.Models;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Gateway.BlindMatch.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        // Standalone MongoDB doesn't support transactions
        Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
    }
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<ProjectProposal> ProjectProposals { get; set; }
    public DbSet<ResearchArea> ResearchAreas { get; set; }
    public DbSet<SupervisorExpertise> SupervisorExpertises { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToCollection("users");

        // MongoDB doesn't support FK joins — relationships are handled manually in controllers
        builder.Entity<ProjectProposal>()
            .Ignore(p => p.ResearchArea)
            .Ignore(p => p.Student)
            .Ignore(p => p.Supervisor);

        builder.Entity<SupervisorExpertise>()
            .HasKey(se => new { se.SupervisorId, se.ResearchAreaId });

        builder.Entity<SupervisorExpertise>()
            .Ignore(se => se.Supervisor)
            .Ignore(se => se.ResearchArea);
    }
}
