using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using SerieA.WebUI.Areas.Admin.Models;

namespace SerieA.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private const string TeamsApiUrl =
            "http://localhost:5046/api/teams";

        private const string MatchesApiUrl =
            "http://localhost:5046/api/matches";

        private const string StandingsApiUrl =
            "http://localhost:5046/api/standings";

        public DashboardController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Genel Bakış";

            var model = new DashboardViewModel();
            var client = _httpClientFactory.CreateClient();

            try
            {
                var teamsTask =
                    client.GetFromJsonAsync<List<TeamViewModel>>(TeamsApiUrl);

                var matchesTask =
                    client.GetFromJsonAsync<List<MatchViewModel>>(MatchesApiUrl);

                var standingsTask =
                    client.GetFromJsonAsync<List<StandingViewModel>>(StandingsApiUrl);

                await Task.WhenAll(teamsTask, matchesTask, standingsTask);

                var teams = await teamsTask ?? new List<TeamViewModel>();
                var matches = await matchesTask ?? new List<MatchViewModel>();
                var standings = await standingsTask ?? new List<StandingViewModel>();

                model.TotalTeams = teams.Count;
                model.TotalMatches = matches.Count;

                model.FinishedMatches = matches.Count(m =>
                    m.Status == "Finished");

                model.LiveMatches = matches
                    .Where(m => m.Status == "Live")
                    .OrderBy(m => m.MatchDate)
                    .ToList();

                model.LiveMatchesCount = model.LiveMatches.Count;

                model.PendingMatches = matches.Count(m =>
                    m.Status == "NotStarted");

                model.RecentMatches = matches
                    .Where(m => m.Status == "Finished")
                    .OrderByDescending(m => m.MatchDate)
                    .Take(5)
                    .ToList();

                model.UpcomingMatches = matches
                    .Where(m => m.Status == "NotStarted"
                        && m.MatchDate >= DateTime.Now)
                    .OrderBy(m => m.MatchDate)
                    .Take(5)
                    .ToList();

                model.TopTeams = standings
                    .OrderBy(s => s.Position)
                    .Take(5)
                    .ToList();
            }
            catch (HttpRequestException)
            {
                TempData["Error"] =
                    "Dashboard verileri alınamadı. API'nin çalıştığını kontrol edin.";
            }
            catch (TaskCanceledException)
            {
                TempData["Error"] =
                    "Dashboard verileri alınırken zaman aşımı oluştu.";
            }

            return View(model);
        }
    }
}
