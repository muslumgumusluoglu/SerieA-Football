using SerieA.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using SerieA.WebUI.Areas.Admin.Models;
using System.Net.Http.Json;

namespace SerieA.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private const string ApiBaseUrl =
            "http://localhost:5046/api/matches";

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(int hafta = 1)
        {
            hafta = Math.Clamp(hafta, 1, 38);

            var model = new FixturePageViewModel
            {
                Week = hafta
            };

            try
            {
                var client = _httpClientFactory.CreateClient();

                model.Matches =
                    await client.GetFromJsonAsync<List<FixtureMatchViewModel>>(
                        $"{ApiBaseUrl}/week/{hafta}") ?? new();
            }
            catch (HttpRequestException)
            {
                model.Error =
                    "Fikstür bilgileri alýnamadý. SerieA.API projesinin çalýþtýðýný kontrol edin.";
            }

            return View(model);
        }

        // Genel kullanýcý maç detay sayfasý
        public async Task<IActionResult> MatchDetails(int id)
        {
            if (id <= 0)
                return BadRequest("Geçersiz maç numarasý.");

            try
            {
                var client = _httpClientFactory.CreateClient();

                var match =
                    await client.GetFromJsonAsync<SerieA.WebUI.Models.MatchDetailViewModel>(
                        $"{ApiBaseUrl}/{id}");

                if (match == null)
                    return NotFound("Maç bulunamadý.");

                return View("MatchDetails", match);
            }
            catch (HttpRequestException)
            {
                return StatusCode(
                    503,
                    "Maç bilgileri alýnamadý. API projesinin çalýþtýðýný kontrol edin.");
            }
            catch (System.Text.Json.JsonException)
            {
                return StatusCode(
                    502,
                    "API'den gelen maç detaylarý okunamadý.");
            }
        }




        public async Task<IActionResult> TeamDetails(int id)
        {
            if (id <= 0)
                return BadRequest("Geçersiz takým numarasý.");

            var client = _httpClientFactory.CreateClient();

            try
            {
                var team = await client.GetFromJsonAsync<TeamDetailViewModel>(
                    $"http://localhost:5046/api/teams/{id}");

                if (team == null)
                    return NotFound("Takým bulunamadý.");

                // Takýmýn lig sýrasý ve puan bilgilerini al
                var standings = await client.GetFromJsonAsync<
                    List<SerieA.WebUI.Models.StandingViewModel>>(
                        "http://localhost:5046/api/standings");

                var standing = standings?.FirstOrDefault(s => s.TeamId == id);

                if (standing != null)
                {
                    team.Position = standing.Position;
                    team.Points = standing.Points;
                    team.Played = standing.Played;
                    team.Won = standing.Won;
                    team.Drawn = standing.Drawn;
                    team.Lost = standing.Lost;
                    team.GoalsFor = standing.GoalsFor;
                    team.GoalsAgainst = standing.GoalsAgainst;
                }

                return View("TeamDetails", team);
            }
            catch (HttpRequestException)
            {
                return StatusCode(
                    503,
                    "Takým bilgileri alýnamadý. SerieA.API projesinin çalýþtýðýný kontrol edin.");
            }
            catch (System.Text.Json.JsonException)
            {
                return StatusCode(
                    502,
                    "API'den gelen takým bilgileri okunamadý.");
            }
        }



        public async Task<IActionResult> Standings()
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var standings =
                    await client.GetFromJsonAsync<List<SerieA.WebUI.Models.StandingViewModel>>(
                        "http://localhost:5046/api/standings");

                return View(standings ?? new List<SerieA.WebUI.Models.StandingViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Puan durumu alýnamadý. SerieA.API projesinin çalýþtýðýný kontrol edin.";

                return View(new List<SerieA.WebUI.Models.StandingViewModel>());
            }
        }



        public IActionResult Privacy() => View();
    }
}
