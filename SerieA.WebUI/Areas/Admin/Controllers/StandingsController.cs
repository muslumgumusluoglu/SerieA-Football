
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using SerieA.WebUI.Areas.Admin.Models;

namespace SerieA.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StandingsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ApiUrl = "http://localhost:5046/api/standings";

        public StandingsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var standings = await client.GetFromJsonAsync<List<StandingViewModel>>(ApiUrl);

                return View(standings ?? new List<StandingViewModel>());
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Puan durumu API'sine bağlanılamadı.";
                return View(new List<StandingViewModel>());
            }
        }
    }
}
