
namespace SerieA.WebUI.Models
{
    public class StatisticsViewModel
    {
        public int TotalTeams { get; set; }
        public int TotalMatches { get; set; }
        public int FinishedMatches { get; set; }
        public int RemainingMatches { get; set; }
        public int TotalGoals { get; set; }
        public double AverageGoals { get; set; }
        public int CompletedWeeks { get; set; }
        public int TotalCards { get; set; }

        public List<TopScorerViewModel> TopScorers { get; set; } = new();
        public List<HighScoringMatchViewModel> HighScoringMatches { get; set; } = new();

        public string? Error { get; set; }


        public List<WeeklyGoalsViewModel> WeeklyGoals { get; set; } = new();

        public List<TeamPerformanceViewModel> TeamPerformances { get; set; } = new();

    }

    public class TopScorerViewModel
    {
        public string PlayerName { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string? TeamLogo { get; set; }
        public int Goals { get; set; }
    }

    public class HighScoringMatchViewModel
    {
        public int Id { get; set; }
        public int Week { get; set; }
        public DateTime MatchDate { get; set; }
        public string HomeTeam { get; set; } = string.Empty;
        public string AwayTeam { get; set; } = string.Empty;
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public int TotalGoals { get; set; }
    }


    public class WeeklyGoalsViewModel
    {
        public int Week { get; set; }
        public int Goals { get; set; }
        public int FinishedMatches { get; set; }
    }

    public class TeamPerformanceViewModel
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public int Played { get; set; }
        public int Won { get; set; }
        public int Drawn { get; set; }
        public int Lost { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
    }

}
