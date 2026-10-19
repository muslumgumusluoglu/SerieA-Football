
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using SerieA.WebUI.Models;

namespace SerieA.WebUI.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private const string ApiBaseUrl =
            "http://localhost:5046/api/statistics";

        public StatisticsController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var model = new StatisticsViewModel();
            var client = _httpClientFactory.CreateClient();

            try
            {
                var overview = await client.GetFromJsonAsync<StatisticsViewModel>(
                    $"{ApiBaseUrl}/overview");

                if (overview != null)
                {
                    model.TotalTeams = overview.TotalTeams;
                    model.TotalMatches = overview.TotalMatches;
                    model.FinishedMatches = overview.FinishedMatches;
                    model.RemainingMatches = overview.RemainingMatches;
                    model.TotalGoals = overview.TotalGoals;
                    model.AverageGoals = overview.AverageGoals;
                    model.CompletedWeeks = overview.CompletedWeeks;
                    model.TotalCards = overview.TotalCards;
                }

                model.TopScorers =
                    await client.GetFromJsonAsync<List<TopScorerViewModel>>(
                        $"{ApiBaseUrl}/top-scorers") ?? new();

                model.HighScoringMatches =
                    await client.GetFromJsonAsync<List<HighScoringMatchViewModel>>(
                        $"{ApiBaseUrl}/high-scoring-matches") ?? new();


                model.WeeklyGoals =
                    await client.GetFromJsonAsync<List<WeeklyGoalsViewModel>>(
                        $"{ApiBaseUrl}/goals-by-week") ?? new();

                model.TeamPerformances =
                    await client.GetFromJsonAsync<List<TeamPerformanceViewModel>>(
                        "http://localhost:5046/api/standings") ?? new();


            }
            catch (HttpRequestException)
            {
                model.Error =
                    "İstatistik verileri alınamadı. SerieA.API projesinin çalıştığını kontrol edin.";
            }
            catch (System.Text.Json.JsonException)
            {
                model.Error =
                    "API verileri okunamadı. JSON alanlarını kontrol edin.";
            }

            return View(model);
        }
    }
}
