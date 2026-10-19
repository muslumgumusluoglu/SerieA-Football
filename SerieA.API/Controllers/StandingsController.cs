
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SerieA.API.Context;
using SerieA.API.DTOs;

namespace SerieA.API.Controllers
{
    [Route("api/standings")]
    [ApiController]
    public class StandingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StandingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/standings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StandingDto>>> GetStandings()
        {
            var teams = await _context.Teams
                .AsNoTracking()
                .ToListAsync();

            var finishedMatches = await _context.Matches
                .AsNoTracking()
                .Where(m => m.Status == "Finished"
                    && m.HomeScore.HasValue
                    && m.AwayScore.HasValue)
                .Select(m => new
                {
                    m.HomeTeamId,
                    m.AwayTeamId,
                    HomeScore = m.HomeScore!.Value,
                    AwayScore = m.AwayScore!.Value
                })
                .ToListAsync();

            var standings = teams.Select(team =>
            {
                var homeMatches = finishedMatches
                    .Where(m => m.HomeTeamId == team.Id)
                    .ToList();

                var awayMatches = finishedMatches
                    .Where(m => m.AwayTeamId == team.Id)
                    .ToList();

                var allMatches = homeMatches
                    .Concat(awayMatches)
                    .ToList();

                int won = 0;
                int drawn = 0;
                int lost = 0;
                int goalsFor = 0;
                int goalsAgainst = 0;

                foreach (var match in homeMatches)
                {
                    goalsFor += match.HomeScore;
                    goalsAgainst += match.AwayScore;

                    if (match.HomeScore > match.AwayScore)
                        won++;
                    else if (match.HomeScore == match.AwayScore)
                        drawn++;
                    else
                        lost++;
                }

                foreach (var match in awayMatches)
                {
                    goalsFor += match.AwayScore;
                    goalsAgainst += match.HomeScore;

                    if (match.AwayScore > match.HomeScore)
                        won++;
                    else if (match.AwayScore == match.HomeScore)
                        drawn++;
                    else
                        lost++;
                }

                return new StandingDto
                {
                    TeamId = team.Id,
                    TeamName = team.Name,
                    LogoUrl = team.LogoUrl,
                    Played = allMatches.Count,
                    Won = won,
                    Drawn = drawn,
                    Lost = lost,
                    GoalsFor = goalsFor,
                    GoalsAgainst = goalsAgainst,
                    Points = (won * 3) + drawn
                };
            })
            .OrderByDescending(t => t.Points)
            .ThenByDescending(t => t.GoalDifference)
            .ThenByDescending(t => t.GoalsFor)
            .ThenBy(t => t.TeamName)
            .ToList();

            for (int i = 0; i < standings.Count; i++)
            {
                standings[i].Position = i + 1;
            }

            return Ok(standings);
        }
    }
}
