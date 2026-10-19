
namespace SerieA.WebUI.Models
{
    public class MatchDetailViewModel
    {
        public int Id { get; set; }

        public int HomeTeamId { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string? HomeTeamLogoUrl { get; set; }

        public int AwayTeamId { get; set; }
        public string AwayTeamName { get; set; } = string.Empty;
        public string? AwayTeamLogoUrl { get; set; }

        public int Week { get; set; }
        public DateTime MatchDate { get; set; }

        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }

        public string Status { get; set; } = string.Empty;
        public string? Stadium { get; set; }

        public List<MatchGoalViewModel> Goals { get; set; } = new();
        public List<MatchCardViewModel> Cards { get; set; } = new();
        public List<SubstitutionViewModel> Substitutions { get; set; } = new();
    }

    public class MatchGoalViewModel
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int Minute { get; set; }
    }

    public class MatchCardViewModel
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int Minute { get; set; }
        public string CardType { get; set; } = string.Empty;
    }

    public class SubstitutionViewModel
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string PlayerIn { get; set; } = string.Empty;
        public string PlayerOut { get; set; } = string.Empty;
        public int Minute { get; set; }
    }
}
