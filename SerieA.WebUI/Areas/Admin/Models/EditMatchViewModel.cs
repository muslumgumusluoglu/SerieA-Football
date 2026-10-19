
using System.ComponentModel.DataAnnotations;

namespace SerieA.WebUI.Areas.Admin.Models
{
    public class EditMatchViewModel
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Ev sahibi takımı seçmelisin.")]
        public int HomeTeamId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Deplasman takımını seçmelisin.")]
        public int AwayTeamId { get; set; }

        [Range(1, 38, ErrorMessage = "Hafta 1 ile 38 arasında olmalıdır.")]
        public int Week { get; set; }

        public DateTime MatchDate { get; set; }

        [Required(ErrorMessage = "Stadyum bilgisi zorunludur.")]
        [StringLength(150)]
        public string Stadium { get; set; } = string.Empty;

        [Required(ErrorMessage = "Maç durumu seçilmelidir.")]
        [RegularExpression(
            "^(NotStarted|Live|Finished|Postponed)$",
            ErrorMessage = "Geçersiz maç durumu.")]
        public string Status { get; set; } = "NotStarted";

    }
}

