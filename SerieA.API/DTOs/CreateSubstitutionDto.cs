
using System.ComponentModel.DataAnnotations;

namespace SerieA.API.DTOs
{
    public class CreateSubstitutionDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Takım seçilmelidir.")]
        public int TeamId { get; set; }

        [Required(ErrorMessage = "Oyuna giren oyuncunun adı zorunludur.")]
        [StringLength(100)]
        public string PlayerIn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Oyundan çıkan oyuncunun adı zorunludur.")]
        [StringLength(100)]
        public string PlayerOut { get; set; } = string.Empty;

        [Range(1, 130, ErrorMessage = "Değişiklik dakikası 1 ile 130 arasında olmalıdır.")]
        public int Minute { get; set; }
    }
}
