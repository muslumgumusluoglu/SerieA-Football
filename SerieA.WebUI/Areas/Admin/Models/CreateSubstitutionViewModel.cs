using System.ComponentModel.DataAnnotations;

namespace SerieA.WebUI.Areas.Admin.Models
{
    public class CreateSubstitutionViewModel
    {
        public int MatchId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Takım seçmelisin.")]
        public int TeamId { get; set; }

        [Required(ErrorMessage = "Oyundan çıkan oyuncu zorunludur.")]
        [StringLength(100)]
        public string PlayerOut { get; set; } = string.Empty;

        [Required(ErrorMessage = "Oyuna giren oyuncu zorunludur.")]
        [StringLength(100)]
        public string PlayerIn { get; set; } = string.Empty;

        [Range(1, 130, ErrorMessage = "Dakika 1 ile 130 arasında olmalıdır.")]
        public int Minute { get; set; }
    }
}
