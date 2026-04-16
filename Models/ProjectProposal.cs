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

        // Foreign Key for ResearchArea
        [Required]
        public int ResearchAreaId { get; set; }
        [ForeignKey("ResearchAreaId")]
        public ResearchArea? ResearchArea { get; set; }

        // Foreign Key for Student (ApplicationUser)
        [Required]
        public string StudentId { get; set; } = string.Empty;
        [ForeignKey("StudentId")]
        public ApplicationUser? Student { get; set; }

        // Foreign Key for Supervisor (ApplicationUser)
        public string? SupervisorId { get; set; }
        [ForeignKey("SupervisorId")]
        public ApplicationUser? Supervisor { get; set; }
    }
}
