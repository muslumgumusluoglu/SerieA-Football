
using System.ComponentModel.DataAnnotations;

namespace SerieA.WebUI.Areas.Admin.Models
{
    public class CreateMatchViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Ev sahibi takımı seçmelisin.")]
        public int HomeTeamId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Deplasman takımını seçmelisin.")]
        public int AwayTeamId { get; set; }

        [Range(1, 38, ErrorMessage = "Hafta 1 ile 38 arasında olmalıdır.")]
        public int Week { get; set; } = 1;

        [Required(ErrorMessage = "Maç tarihini girmelisin.")]
        public DateTime MatchDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Stadyum bilgisi zorunludur.")]
        [StringLength(150, ErrorMessage = "Stadyum adı en fazla 150 karakter olabilir.")]
        public string Stadium { get; set; } = string.Empty;
    }
}
