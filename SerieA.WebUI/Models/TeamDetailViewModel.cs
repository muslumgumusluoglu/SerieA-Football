
namespace SerieA.WebUI.Models
{
    public class TeamDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string City { get; set; } = string.Empty;
        public string Stadium { get; set; } = string.Empty;

        // Lig istatistikleri
        public int Position { get; set; }
        public int Points { get; set; }
        public int Played { get; set; }
        public int Won { get; set; }
        public int Drawn { get; set; }
        public int Lost { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference => GoalsFor - GoalsAgainst;

        // Takımın maçları
        public List<TeamMatchViewModel> Matches { get; set; } = new();
    }

    public class TeamMatchViewModel
    {
        public int Id { get; set; }
        public int Week { get; set; }
        public DateTime MatchDate { get; set; }

        public int HomeTeamId { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string? HomeTeamLogoUrl { get; set; }

        public int AwayTeamId { get; set; }
        public string AwayTeamName { get; set; } = string.Empty;
        public string? AwayTeamLogoUrl { get; set; }

        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }

        public string Status { get; set; } = string.Empty;
        public string Stadium { get; set; } = string.Empty;
    }
}
