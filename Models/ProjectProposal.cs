using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gateway.BlindMatch.Models
{
    public class ProjectProposal
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Abstract { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string TechnicalStack { get; set; } = string.Empty;

        [Required]
        public ProjectStatus Status { get; set; } = ProjectStatus.Pending;

        // Research Area reference
        [Required]
        public int ResearchAreaId { get; set; }

        [NotMapped]
        public ResearchArea? ResearchArea { get; set; }

        // Student reference (stored as string ID)
        [Required]
        public string StudentId { get; set; } = string.Empty;

        [NotMapped]
        public ApplicationUser? Student { get; set; }

        // Supervisor reference (stored as string ID)
        public string? SupervisorId { get; set; }

        [NotMapped]
        public ApplicationUser? Supervisor { get; set; }
    }
}
