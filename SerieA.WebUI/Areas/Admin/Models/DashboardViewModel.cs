using SerieA.WebUI.Areas.Admin.Models;

namespace SerieA.WebUI.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        // Genel istatistikler
        public int TotalTeams { get; set; }
        public int TotalMatches { get; set; }
        public int FinishedMatches { get; set; }
        public int PendingMatches { get; set; }
        public int LiveMatchesCount { get; set; }

        // Canlı maçlar
        public List<MatchViewModel> LiveMatches { get; set; } = new();

        // Sonuçlanan son maçlar
        public List<MatchViewModel> RecentMatches { get; set; } = new();

        // Yaklaşan maçlar
        public List<MatchViewModel> UpcomingMatches { get; set; } = new();

        // Puan durumundaki ilk 5 takım
        public List<StandingViewModel> TopTeams { get; set; } = new();
    }
}