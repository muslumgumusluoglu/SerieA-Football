
using System.Text.Json.Serialization;

namespace SerieA.WebUI.Areas.Admin.Models
{
    public class MatchViewModel
    {
        public int Id { get; set; }

        public int HomeTeamId { get; set; }
        public TeamInfoViewModel? HomeTeam { get; set; }

        public int AwayTeamId { get; set; }
        public TeamInfoViewModel? AwayTeam { get; set; }

        public int Week { get; set; }
        public DateTime MatchDate { get; set; }

        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }

        public string Status { get; set; } = "NotStarted";
        public string Stadium { get; set; } = string.Empty;

        [JsonIgnore]
        public string HomeTeamName => HomeTeam?.Name ?? "Bilinmiyor";

        [JsonIgnore]
        public string? HomeTeamLogoUrl => HomeTeam?.LogoUrl;

        [JsonIgnore]
        public string AwayTeamName => AwayTeam?.Name ?? "Bilinmiyor";

        [JsonIgnore]
        public string? AwayTeamLogoUrl => AwayTeam?.LogoUrl;
    }

    public class TeamInfoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
    }
}
