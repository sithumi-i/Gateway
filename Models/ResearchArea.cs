using System.ComponentModel.DataAnnotations;

namespace Gateway.BlindMatch.Models
{
    public class ResearchArea
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
