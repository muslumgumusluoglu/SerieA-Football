
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SerieA.API.Context;
using SerieA.API.Entities;

namespace SerieA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeamsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/teams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            var teams = await _context.Teams
                .AsNoTracking()
                .OrderBy(t => t.Name)
                .ToListAsync();

            return Ok(teams);
        }

        // POST: api/teams
        [HttpPost]
        public async Task<ActionResult<Team>> CreateTeam(Team team)
        {
            if (team == null || string.IsNullOrWhiteSpace(team.Name))
            {
                return BadRequest("Takım adı boş olamaz.");
            }

            var teamName = team.Name.Trim();

            var teamExists = await _context.Teams
                .AnyAsync(t => t.Name.ToLower() == teamName.ToLower());

            if (teamExists)
            {
                return Conflict("Bu takım zaten kayıtlı.");
            }

            team.Name = teamName;

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTeams),
                new { id = team.Id },
                team);
        }

        // PUT: api/teams/21
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, Team updatedTeam)
        {
            if (id != updatedTeam.Id)
            {
                return BadRequest("Takım ID bilgileri uyuşmuyor.");
            }

            if (string.IsNullOrWhiteSpace(updatedTeam.Name))
            {
                return BadRequest("Takım adı boş olamaz.");
            }

            var team = await _context.Teams.FindAsync(id);

            if (team == null)
            {
                return NotFound("Takım bulunamadı.");
            }

            var teamName = updatedTeam.Name.Trim();

            var duplicateExists = await _context.Teams
                .AnyAsync(t => t.Id != id &&
                               t.Name.ToLower() == teamName.ToLower());

            if (duplicateExists)
            {
                return Conflict("Bu takım adı zaten kullanılıyor.");
            }

            team.Name = teamName;

            await _context.SaveChangesAsync();

            return NoContent();
        }


        // GET: api/teams/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTeamById(int id)
        {
            var team = await _context.Teams
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
            {
                return NotFound("Takım bulunamadı.");
            }

            var matches = await _context.Matches
                .AsNoTracking()
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.HomeTeamId == id || m.AwayTeamId == id)
                .OrderBy(m => m.MatchDate)
                .Select(m => new
                {
                    m.Id,
                    m.Week,
                    m.MatchDate,
                    m.HomeTeamId,
                    HomeTeamName = m.HomeTeam!.Name,
                    HomeTeamLogoUrl = m.HomeTeam.LogoUrl,
                    m.AwayTeamId,
                    AwayTeamName = m.AwayTeam!.Name,
                    AwayTeamLogoUrl = m.AwayTeam.LogoUrl,
                    m.HomeScore,
                    m.AwayScore,
                    m.Status,
                    m.Stadium
                })
                .ToListAsync();

            return Ok(new
            {
                team.Id,
                team.Name,
                team.LogoUrl,
                team.City,
                team.Stadium,
                Matches = matches
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
                return NotFound("Takım bulunamadı.");

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
