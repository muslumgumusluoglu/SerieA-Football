using System.ComponentModel.DataAnnotations;

namespace SerieA.API.DTOs
{
    public class CreateMatchDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Ev sahibi takım seçilmelidir.")]
        public int HomeTeamId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Deplasman takımı seçilmelidir.")]
        public int AwayTeamId { get; set; }

        [Range(1, 38, ErrorMessage = "Hafta 1 ile 38 arasında olmalıdır.")]
        public int Week { get; set; }

        public DateTime MatchDate { get; set; }

        [Required(ErrorMessage = "Stadyum bilgisi zorunludur.")]
        [StringLength(150)]
        public string Stadium { get; set; } = string.Empty;
    }
}
