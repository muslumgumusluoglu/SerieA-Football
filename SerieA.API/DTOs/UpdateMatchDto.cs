
using System.ComponentModel.DataAnnotations;

namespace SerieA.API.DTOs
{
    public class UpdateMatchDto
    {
        [Range(1, int.MaxValue)]
        public int HomeTeamId { get; set; }

        [Range(1, int.MaxValue)]
        public int AwayTeamId { get; set; }

        [Range(1, 38)]
        public int Week { get; set; }

        public DateTime MatchDate { get; set; }

        [Required]
        [StringLength(150)]
        public string Stadium { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(NotStarted|Live|Finished|Postponed)$",
            ErrorMessage = "Geçersiz maç durumu.")]
        public string Status { get; set; } = "NotStarted";

        [Range(0, 100)]
        public int? HomeScore { get; set; }

        [Range(0, 100)]
        public int? AwayScore { get; set; }
    }
}
