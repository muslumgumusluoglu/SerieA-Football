using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using SerieA.WebUI.Areas.Admin.Models;

namespace SerieA.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FixturesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private const string ApiUrl = "http://localhost:5046/api/matches";

        public FixturesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(int week = 1)
        {
            if (week < 1 || week > 38)
                week = 1;

            var client = _httpClientFactory.CreateClient();

            try
            {
                var matches = await client.GetFromJsonAsync<List<MatchViewModel>>(
                    $"{ApiUrl}/week/{week}");

                ViewBag.Week = week;

                return View(matches ?? new List<MatchViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.Week = week;
                TempData["Error"] = "Fikstür API'sine bağlanılamadı.";
                return View(new List<MatchViewModel>());
            }
        }
    }
}
