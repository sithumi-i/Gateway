using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gateway.BlindMatch.Models
{
    public class SupervisorExpertise
    {
        [Required]
        public string SupervisorId { get; set; } = string.Empty;

        [NotMapped]
        public ApplicationUser? Supervisor { get; set; }

        [Required]
        public int ResearchAreaId { get; set; }

        [NotMapped]
        public ResearchArea? ResearchArea { get; set; }
    }
}
