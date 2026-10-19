
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using SerieA.WebUI.Areas.Admin.Models;

namespace SerieA.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TeamsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ApiUrl = "http://localhost:5046/api/teams";

        public TeamsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Takım listesi
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var teams = await client.GetFromJsonAsync<List<TeamViewModel>>(ApiUrl);

                return View(teams ?? new List<TeamViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "API'ye bağlanılamadı. SerieA.API projesinin çalıştığını kontrol edin.";
                return View(new List<TeamViewModel>());
            }
        }

        // Yeni takım formu
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "Yeni Takım Ekle";
            return View(new TeamViewModel());
        }

        // Yeni takım kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeamViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Takım adı boş olamaz.");
                ViewData["Title"] = "Yeni Takım Ekle";
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.PostAsJsonAsync(ApiUrl, model);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    ModelState.AddModelError(nameof(model.Name), "Bu takım zaten kayıtlı.");
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        $"Takım kaydedilemedi. API yanıt kodu: {(int)response.StatusCode}");
                }
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "API'ye bağlanılamadı. SerieA.API çalışıyor mu kontrol edin.");
            }

            ViewData["Title"] = "Yeni Takım Ekle";
            return View(model);
        }




        // Takım düzenleme formunu aç
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var teams = await client.GetFromJsonAsync<List<TeamViewModel>>(ApiUrl);

                var team = teams?.FirstOrDefault(t => t.Id == id);

                if (team == null)
                {
                    return NotFound("Takım bulunamadı.");
                }

                ViewData["Title"] = "Takım Düzenle";
                return View(team);
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, "API'ye bağlanılamadı.");
            }
        }

        // Takım değişikliklerini kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeamViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest("Takım ID bilgileri uyuşmuyor.");
            }

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Takım adı boş olamaz.");
                ViewData["Title"] = "Takım Düzenle";
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.PutAsJsonAsync(
                    $"{ApiUrl}/{id}",
                    model);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    ModelState.AddModelError(nameof(model.Name), "Bu takım adı zaten kullanılıyor.");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound("Takım bulunamadı.");
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        $"Takım güncellenemedi. API yanıt kodu: {(int)response.StatusCode}");
                }
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "API'ye bağlanılamadı.");
            }

            ViewData["Title"] = "Takım Düzenle";
            return View(model);
        }



        // Takım detay sayfası
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest("Geçersiz takım numarası.");

            var client = _httpClientFactory.CreateClient();

            try
            {
                // Takım bilgilerini ve maçlarını API'den al
                var team = await client.GetFromJsonAsync<
                    SerieA.WebUI.Models.TeamDetailViewModel>(
                    $"{ApiUrl}/{id}");

                if (team == null)
                    return NotFound("Takım bulunamadı.");

                // Lig sıralamasını ve istatistiklerini al
                var standings = await client.GetFromJsonAsync<
                    List<SerieA.WebUI.Models.StandingViewModel>>(
                    "http://localhost:5046/api/standings");

                var standing = standings?
                    .FirstOrDefault(s => s.TeamId == id);

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

                ViewData["Title"] = $"{team.Name} - Takım Detayı";

                return View(team);
            }
            catch (HttpRequestException)
            {
                return StatusCode(
                    503,
                    "API'ye bağlanılamadı. SerieA.API projesinin çalıştığını kontrol edin.");
            }
            catch (System.Text.Json.JsonException)
            {
                return StatusCode(
                    502,
                    "API'den gelen takım bilgileri okunamadı.");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.DeleteAsync($"{ApiUrl}/{id}");

                if (response.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound("Takım bulunamadı.");

                TempData["Error"] =
                    $"Takım silinemedi. API yanıt kodu: {(int)response.StatusCode}";

                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye bağlanılamadı. SerieA.API çalışıyor mu?";
                return RedirectToAction(nameof(Index));
            }
        }


    }
}
