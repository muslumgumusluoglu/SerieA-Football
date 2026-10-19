using System.Text.Json.Serialization;

namespace SerieA.WebUI.Models
{
    public class FixturePageViewModel
    {
        public int Week { get; set; } = 1;
        public List<FixtureMatchViewModel> Matches { get; set; } = new();
        public string? Error { get; set; }
    }

    public class FixtureMatchViewModel
    {
        public int Id { get; set; }
        public int Week { get; set; }
        public DateTime MatchDate { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string Status { get; set; } = "NotStarted";
        public string Stadium { get; set; } = "";
        public FixtureTeamViewModel? HomeTeam { get; set; }
        public FixtureTeamViewModel? AwayTeam { get; set; }
    }

    public class FixtureTeamViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? LogoUrl { get; set; }
        public string? Stadium { get; set; }
    }
}
