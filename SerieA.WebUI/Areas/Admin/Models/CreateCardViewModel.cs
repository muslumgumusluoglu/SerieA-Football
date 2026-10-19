
using System.ComponentModel.DataAnnotations;

namespace SerieA.WebUI.Areas.Admin.Models
{
    public class CreateCardViewModel
    {
        public int MatchId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Takım seçmelisin.")]
        public int TeamId { get; set; }

        [Required(ErrorMessage = "Oyuncu adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Oyuncu adı en fazla 100 karakter olabilir.")]
        public string PlayerName { get; set; } = string.Empty;

        [Range(1, 130, ErrorMessage = "Dakika 1 ile 130 arasında olmalıdır.")]
        public int Minute { get; set; }

        [Required(ErrorMessage = "Kart türü seçilmelidir.")]
        [RegularExpression("^(Yellow|Red)$",
            ErrorMessage = "Geçerli bir kart türü seçmelisin.")]
        public string CardType { get; set; } = "Yellow";
    }
}
