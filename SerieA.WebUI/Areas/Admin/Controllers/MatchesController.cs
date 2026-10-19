using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using SerieA.WebUI.Areas.Admin.Models;

namespace SerieA.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MatchesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private const string ApiUrl =
            "http://localhost:5046/api/matches";

        public MatchesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Haftaya göre maç listesi
        [HttpGet]
        public async Task<IActionResult> Index(int week = 1)
        {
            if (week < 1 || week > 38)
            {
                week = 1;
            }

            var client = _httpClientFactory.CreateClient();

            try
            {
                var matches =
                    await client.GetFromJsonAsync<List<MatchViewModel>>(
                        $"{ApiUrl}/week/{week}");

                ViewBag.Week = week;
                return View(matches ?? new List<MatchViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.Week = week;
                ViewBag.Error =
                    "API'ye bağlanılamadı. SerieA.API projesinin çalıştığını kontrol edin.";

                return View(new List<MatchViewModel>());
            }
        }


        // Yeni maç formunu aç
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var success = await TakimlariYukle();

            if (!success)
            {
                TempData["Error"] =
                    "Takım listesi alınamadı. API projesinin çalıştığını kontrol edin.";

                return RedirectToAction(nameof(Index));
            }

            return View(new CreateMatchViewModel
            {
                Week = 1,
                MatchDate = DateTime.Now
            });
        }

        // Yeni maçı kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMatchViewModel model)
        {
            if (model.HomeTeamId == model.AwayTeamId)
            {
                ModelState.AddModelError(
                    nameof(model.AwayTeamId),
                    "Ev sahibi ve deplasman takımı aynı olamaz.");
            }

            if (!ModelState.IsValid)
            {
                await TakimlariYukle();
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.PostAsJsonAsync(ApiUrl, model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Maç başarıyla eklendi.";

                    return RedirectToAction(
                        nameof(Index),
                        new { week = model.Week });
                }

                var apiError = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(
                    string.Empty,
                    $"Maç kaydedilemedi. API kodu: {(int)response.StatusCode}. {apiError}");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "API'ye bağlanılamadı. SerieA.API çalışıyor mu?");
            }

            await TakimlariYukle();
            return View(model);
        }

        // Takımları formdaki açılır listelere yükle
        private async Task<bool> TakimlariYukle()
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var teams = await client.GetFromJsonAsync<List<TeamViewModel>>(
                    "http://localhost:5046/api/teams");

                ViewBag.Teams = teams ?? new List<TeamViewModel>();

                return true;
            }
            catch (HttpRequestException)
            {
                ViewBag.Teams = new List<TeamViewModel>();
                return false;
            }
        }



        // Maç düzenleme formunu aç
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var match = await client.GetFromJsonAsync<MatchViewModel>(
                    $"{ApiUrl}/{id}");

                if (match == null)
                    return NotFound("Maç bulunamadı.");

                var success = await TakimlariYukle();

                if (!success)
                    return StatusCode(503, "Takım listesi alınamadı.");

                var model = new EditMatchViewModel
                {
                    Id = match.Id,
                    HomeTeamId = match.HomeTeamId,
                    AwayTeamId = match.AwayTeamId,
                    Week = match.Week,
                    MatchDate = match.MatchDate,
                    Stadium = match.Stadium,
                    Status = match.Status
                };

                return View(model);
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, "API'ye bağlanılamadı.");
            }
        }

        // Maç değişikliklerini kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditMatchViewModel model)
        {
            if (id != model.Id)
                return BadRequest("Maç ID bilgileri uyuşmuyor.");

            if (model.HomeTeamId == model.AwayTeamId)
            {
                ModelState.AddModelError(
                    nameof(model.AwayTeamId),
                    "Ev sahibi ve deplasman takımı aynı olamaz.");
            }

            if (!ModelState.IsValid)
            {
                await TakimlariYukle();
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
                    TempData["Success"] = "Maç başarıyla güncellendi.";

                    return RedirectToAction(
                        nameof(Index),
                        new { week = model.Week });
                }

                var apiError = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(
                    string.Empty,
                    $"Maç güncellenemedi. API kodu: {(int)response.StatusCode}. {apiError}");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "API'ye bağlanılamadı. SerieA.API çalışıyor mu?");
            }

            await TakimlariYukle();
            return View(model);
        }



        // Maçı sil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int week)
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.DeleteAsync($"{ApiUrl}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Maç başarıyla silindi.";
                }
                else if (response.StatusCode ==
                         System.Net.HttpStatusCode.NotFound)
                {
                    TempData["Error"] = "Silinecek maç bulunamadı.";
                }
                else
                {
                    var apiError = await response.Content.ReadAsStringAsync();

                    TempData["Error"] =
                        $"Maç silinemedi. API kodu: {(int)response.StatusCode}. {apiError}";
                }
            }
            catch (HttpRequestException)
            {
                TempData["Error"] =
                    "API'ye bağlanılamadı. SerieA.API çalışıyor mu?";
            }

            return RedirectToAction(
                nameof(Index),
                new { week = week });
        }

        // Maç detay ekranı
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest("Geçersiz maç numarası.");

            var client = _httpClientFactory.CreateClient();

            try
            {
                var match = await client.GetFromJsonAsync<MatchDetailViewModel>(
                    $"{ApiUrl}/{id}");

                if (match == null)
                    return NotFound("Maç bulunamadı.");

                return View(match);
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
                    "API'den gelen maç detayları okunamadı.");
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGoal(
            CreateGoalViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Gol bilgilerini kontrol et.";
                return RedirectToAction(
                    nameof(Details),
                    new { id = model.MatchId });
            }

            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.PostAsJsonAsync(
                    $"{ApiUrl}/{model.MatchId}/goals",
                    model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Gol başarıyla eklendi.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Gol eklenemedi: {error}";
                }
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye bağlanılamadı.";
            }

            return RedirectToAction(
                nameof(Details),
                new { id = model.MatchId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGoal(int matchId, int goalId)
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.DeleteAsync(
                    $"{ApiUrl}/{matchId}/goals/{goalId}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Gol başarıyla silindi.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Gol silinemedi: {error}";
                }
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye bağlanılamadı.";
            }

            return RedirectToAction(nameof(Details), new { id = matchId });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCard(int matchId, int cardId)
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.DeleteAsync(
                    $"{ApiUrl}/{matchId}/cards/{cardId}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Kart başarıyla silindi.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Kart silinemedi: {error}";
                }
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye bağlanılamadı.";
            }

            return RedirectToAction(nameof(Details), new { id = matchId });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubstitution(
            int matchId,
            int substitutionId)
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.DeleteAsync(
                    $"{ApiUrl}/{matchId}/substitutions/{substitutionId}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] =
                        "Oyuncu değişikliği başarıyla silindi.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();

                    TempData["Error"] =
                        $"Oyuncu değişikliği silinemedi: {error}";
                }
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye bağlanılamadı.";
            }

            return RedirectToAction(
                nameof(Details),
                new { id = matchId });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCard(
            CreateCardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Kart bilgilerini kontrol et.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = model.MatchId });
            }

            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.PostAsJsonAsync(
                    $"{ApiUrl}/{model.MatchId}/cards",
                    model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Kart başarıyla eklendi.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();

                    TempData["Error"] = $"Kart eklenemedi: {error}";
                }
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye bağlanılamadı.";
            }

            return RedirectToAction(
                nameof(Details),
                new { id = model.MatchId });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSubstitution(
            CreateSubstitutionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Oyuncu değişikliği bilgilerini kontrol et.";
                return RedirectToAction(
                    nameof(Details),
                    new { id = model.MatchId });
            }

            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.PostAsJsonAsync(
                    $"{ApiUrl}/{model.MatchId}/substitutions",
                    model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] =
                        "Oyuncu değişikliği başarıyla eklendi.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();

                    TempData["Error"] =
                        $"Oyuncu değişikliği eklenemedi: {error}";
                }
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "API'ye bağlanılamadı.";
            }

            return RedirectToAction(
                nameof(Details),
                new { id = model.MatchId });
        }



    }
}
