using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Gateway.BlindMatch.Models;

namespace Gateway.BlindMatch.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<ProjectProposal> ProjectProposals { get; set; }
    public DbSet<ResearchArea> ResearchAreas { get; set; }
    public DbSet<SupervisorExpertise> SupervisorExpertises { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Define composite primary key for SupervisorExpertise
        builder.Entity<SupervisorExpertise>()
            .HasKey(se => new { se.SupervisorId, se.ResearchAreaId });
    }
}
