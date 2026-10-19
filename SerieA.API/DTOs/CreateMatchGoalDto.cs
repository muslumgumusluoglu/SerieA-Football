using System.ComponentModel.DataAnnotations;

namespace SerieA.API.DTOs
{
    public class CreateMatchGoalDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Takım seçilmelidir.")]
        public int TeamId { get; set; }

        [Required(ErrorMessage = "Oyuncu adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Oyuncu adı en fazla 100 karakter olabilir.")]
        public string PlayerName { get; set; } = string.Empty;

        [Range(1, 130, ErrorMessage = "Gol dakikası 1 ile 130 arasında olmalıdır.")]
        public int Minute { get; set; }
    }
}
