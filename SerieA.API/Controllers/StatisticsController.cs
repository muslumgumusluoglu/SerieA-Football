
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SerieA.API.Context;

namespace SerieA.API.Controllers
{
    [Route("api/statistics")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StatisticsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/statistics/overview
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var matches = await _context.Matches
                .AsNoTracking()
                .Where(m => m.Status == "Finished"
                    && m.HomeScore.HasValue
                    && m.AwayScore.HasValue)
                .Select(m => new
                {
                    m.Week,
                    HomeScore = m.HomeScore!.Value,
                    AwayScore = m.AwayScore!.Value
                })
                .ToListAsync();

            var totalGoals = matches.Sum(m =>
                m.HomeScore + m.AwayScore);

            var totalCards = await _context.MatchCards
                .AsNoTracking()
                .CountAsync(c =>
                    _context.Matches.Any(m =>
                        m.Id == c.MatchId &&
                        m.Status == "Finished"));

            return Ok(new
            {
                TotalTeams = await _context.Teams.CountAsync(),
                TotalMatches = await _context.Matches.CountAsync(),
                FinishedMatches = matches.Count,
                RemainingMatches =
                    await _context.Matches.CountAsync(
                        m => m.Status == "NotStarted"),
                TotalGoals = totalGoals,
                AverageGoals = matches.Count == 0
                    ? 0
                    : Math.Round(
                        (double)totalGoals / matches.Count, 2),
                CompletedWeeks = matches
                    .Select(m => m.Week)
                    .Distinct()
                    .Count(),
                TotalCards = totalCards
            });
        }

        // GET: api/statistics/top-scorers
        [HttpGet("top-scorers")]
        public async Task<IActionResult> GetTopScorers(
            int take = 10)
        {
            take = Math.Clamp(take, 1, 50);

            var finishedMatchIds = _context.Matches
                .AsNoTracking()
                .Where(m => m.Status == "Finished"
                    && m.HomeScore.HasValue
                    && m.AwayScore.HasValue)
                .Select(m => m.Id);

            var scorers = await _context.MatchGoals
                .AsNoTracking()
                .Where(g => finishedMatchIds.Contains(g.MatchId))
                .GroupBy(g => new
                {
                    g.PlayerName,
                    g.TeamId,
                    TeamName = g.Team!.Name,
                    TeamLogo = g.Team!.LogoUrl
                })
                .Select(g => new
                {
                    PlayerName = g.Key.PlayerName,
                    TeamId = g.Key.TeamId,
                    TeamName = g.Key.TeamName,
                    TeamLogo = g.Key.TeamLogo,
                    Goals = g.Count()
                })
                .OrderByDescending(g => g.Goals)
                .ThenBy(g => g.PlayerName)
                .Take(take)
                .ToListAsync();

            return Ok(scorers);
        }

        // GET: api/statistics/high-scoring-matches
        [HttpGet("high-scoring-matches")]
        public async Task<IActionResult> GetHighScoringMatches(
            int take = 10)
        {
            take = Math.Clamp(take, 1, 50);

            var matches = await _context.Matches
                .AsNoTracking()
                .Where(m => m.Status == "Finished"
                    && m.HomeScore.HasValue
                    && m.AwayScore.HasValue)
                .Select(m => new
                {
                    m.Id,
                    m.Week,
                    m.MatchDate,
                    HomeTeam = m.HomeTeam!.Name,
                    AwayTeam = m.AwayTeam!.Name,
                    HomeScore = m.HomeScore!.Value,
                    AwayScore = m.AwayScore!.Value,
                    TotalGoals =
                        m.HomeScore!.Value + m.AwayScore!.Value
                })
                .OrderByDescending(m => m.TotalGoals)
                .ThenBy(m => m.MatchDate)
                .Take(take)
                .ToListAsync();

            return Ok(matches);
        }



        [HttpGet("goals-by-week")]
        public async Task<IActionResult> GetGoalsByWeek()
        {
            var matches = await _context.Matches
                .AsNoTracking()
                .Where(m => m.Status == "Finished"
                    && m.HomeScore.HasValue
                    && m.AwayScore.HasValue)
                .Select(m => new
                {
                    m.Week,
                    HomeScore = m.HomeScore!.Value,
                    AwayScore = m.AwayScore!.Value
                })
                .ToListAsync();

            var result = Enumerable.Range(1, 38)
                .Select(week =>
                {
                    var weekMatches = matches
                        .Where(m => m.Week == week)
                        .ToList();

                    return new
                    {
                        Week = week,
                        Goals = weekMatches.Sum(m =>
                            m.HomeScore + m.AwayScore),
                        FinishedMatches = weekMatches.Count
                    };
                })
                .ToList();

            return Ok(result);
        }

    }
}
