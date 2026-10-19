using System.Net.Http.Json;
using System.Text.Json;
using SerieA.Mobile.Models;

namespace SerieA.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5046/"),
            Timeout = TimeSpan.FromSeconds(15)
        };
    }

    public async Task<List<MatchDto>> GetMatchesByWeekAsync(int week)
    {
        var matches = await _httpClient.GetFromJsonAsync<List<MatchDto>>(
            $"api/matches/week/{week}");

        return matches ?? new List<MatchDto>();
    }

    public async Task<List<StandingDto>> GetStandingsAsync()
    {
        var standings = await _httpClient.GetFromJsonAsync<List<StandingDto>>(
            "api/standings");

        return standings ?? new List<StandingDto>();
    }

    public async Task<JsonElement> GetStatisticsOverviewAsync()
    {
        return await GetStatisticsAsync("api/statistics/overview");
    }

    public async Task<JsonElement> GetTopScorersAsync()
    {
        return await GetStatisticsAsync("api/statistics/top-scorers");
    }

    public async Task<JsonElement> GetHighScoringMatchesAsync()
    {
        return await GetStatisticsAsync("api/statistics/high-scoring-matches");
    }

    public async Task<JsonElement> GetGoalsByWeekAsync()
    {
        return await GetStatisticsAsync("api/statistics/goals-by-week");
    }

    private async Task<JsonElement> GetStatisticsAsync(string endpoint)
    {
        return await _httpClient.GetFromJsonAsync<JsonElement>(endpoint);
    }
}