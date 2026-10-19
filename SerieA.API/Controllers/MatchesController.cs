using SerieA.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SerieA.API.Context;
using SerieA.API.Entities;

namespace SerieA.API.Controllers
{
    [Route("api/matches")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MatchesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/matches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Match>>> GetAllMatches()
        {
            var matches = await _context.Matches
                .AsNoTracking()
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .OrderBy(m => m.MatchDate)
                .ToListAsync();

            return Ok(matches);
        }

        // GET: api/matches/week/1
        [HttpGet("week/{week:int}")]
        public async Task<ActionResult<IEnumerable<Match>>> GetMatchesByWeek(int week)
        {
            if (week < 1)
                return BadRequest("Hafta numarası 1 veya daha büyük olmalıdır.");

            var matches = await _context.Matches
                .AsNoTracking()
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.Week == week)
                .OrderBy(m => m.MatchDate)
                .ToListAsync();

            return Ok(matches);
        }




        [HttpGet("{id:int}")]
        public async Task<ActionResult<MatchDetailDto>> GetMatchById(int id)
        {
            var match = await _context.Matches
                .AsNoTracking()
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
                return NotFound("Maç bulunamadı.");


            var goals = await _context.MatchGoals
                .AsNoTracking()
                .Where(g => g.MatchId == id)
                .OrderBy(g => g.Minute)
                .Select(g => new MatchGoalDto
                {
                    Id = g.Id,
                    TeamId = g.TeamId,
                    PlayerName = g.PlayerName,
                    Minute = g.Minute
                })
                .ToListAsync();



            var cards = await _context.MatchCards
                .AsNoTracking()
                .Where(c => c.MatchId == id)
                .OrderBy(c => c.Minute)
                .Select(c => new MatchCardDto
                {
                    Id = c.Id,
                    TeamId = c.TeamId,
                    PlayerName = c.PlayerName,
                    Minute = c.Minute,
                    CardType = c.CardType
                })
                .ToListAsync();



            var substitutions = await _context.Substitutions
                .AsNoTracking()
                .Where(s => s.MatchId == id)
                .OrderBy(s => s.Minute)
                .Select(s => new SubstitutionDto
                {
                    Id = s.Id,
                    TeamId = s.TeamId,
                    PlayerIn = s.PlayerIn,
                    PlayerOut = s.PlayerOut,
                    Minute = s.Minute
                })
                .ToListAsync();


            var result = new MatchDetailDto
            {
                Id = match.Id,
                HomeTeamId = match.HomeTeamId,
                HomeTeamName = match.HomeTeam?.Name ?? string.Empty,
                HomeTeamLogoUrl = match.HomeTeam?.LogoUrl,

                AwayTeamId = match.AwayTeamId,
                AwayTeamName = match.AwayTeam?.Name ?? string.Empty,
                AwayTeamLogoUrl = match.AwayTeam?.LogoUrl,

                Week = match.Week,
                MatchDate = match.MatchDate,
                HomeScore = match.HomeScore,
                AwayScore = match.AwayScore,
                Status = match.Status,
                Stadium = match.Stadium,

                Goals = goals,
                Cards = cards,
                Substitutions = substitutions
            };

            return Ok(result);
        }




        // POST: api/matches
        [HttpPost]
        public async Task<ActionResult<Match>> CreateMatch(
            [FromBody] CreateMatchDto dto)
        {
            // Ev sahibi ve deplasman takımı aynı olamaz.
            if (dto.HomeTeamId == dto.AwayTeamId)
            {
                return BadRequest(
                    "Ev sahibi ve deplasman takımı aynı olamaz.");
            }

            // İki takım da veritabanında bulunmalı.
            var teamIds = await _context.Teams
                .Where(t => t.Id == dto.HomeTeamId ||
                            t.Id == dto.AwayTeamId)
                .Select(t => t.Id)
                .ToListAsync();

            if (teamIds.Count != 2)
            {
                return BadRequest(
                    "Ev sahibi veya deplasman takımı bulunamadı.");
            }

            // Bir takım aynı haftada yalnızca bir maç oynayabilir.
            var teamAlreadyScheduled = await _context.Matches
                .AnyAsync(m =>
                    m.Week == dto.Week &&
                    (m.HomeTeamId == dto.HomeTeamId ||
                     m.AwayTeamId == dto.HomeTeamId ||
                     m.HomeTeamId == dto.AwayTeamId ||
                     m.AwayTeamId == dto.AwayTeamId));

            if (teamAlreadyScheduled)
            {
                return BadRequest(
                    "Bu takımlardan biri seçilen haftada zaten bir maça eklenmiş.");
            }

            var match = new Match
            {
                HomeTeamId = dto.HomeTeamId,
                AwayTeamId = dto.AwayTeamId,
                Week = dto.Week,
                MatchDate = dto.MatchDate,
                Stadium = dto.Stadium.Trim(),
                Status = "NotStarted",
                HomeScore = null,
                AwayScore = null
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMatchById),
                new { id = match.Id },
                match);
        }





        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMatch(
            int id, [FromBody] UpdateMatchDto dto)
        {
            var match = await _context.Matches
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
                return NotFound("Maç bulunamadı.");

            // Takımlar aynı olamaz.
            if (dto.HomeTeamId == dto.AwayTeamId)
                return BadRequest(
                    "Ev sahibi ve deplasman takımı aynı olamaz.");

            // Hafta numarası geçerli olmalı.
            if (dto.Week < 1 || dto.Week > 38)
                return BadRequest(
                    "Hafta numarası 1 ile 38 arasında olmalıdır.");

            // Takımlar veritabanında bulunmalı.
            var teamIds = await _context.Teams
                .Where(t => t.Id == dto.HomeTeamId ||
                            t.Id == dto.AwayTeamId)
                .Select(t => t.Id)
                .ToListAsync();

            if (teamIds.Count != 2)
                return BadRequest("Takımlardan biri bulunamadı.");

            // Aynı takım aynı haftada başka bir maçta oynayamaz.
            var conflict = await _context.Matches.AnyAsync(m =>
                m.Id != id &&
                m.Week == dto.Week &&
                (m.HomeTeamId == dto.HomeTeamId ||
                 m.AwayTeamId == dto.HomeTeamId ||
                 m.HomeTeamId == dto.AwayTeamId ||
                 m.AwayTeamId == dto.AwayTeamId));

            if (conflict)
                return BadRequest(
                    "Takımlardan biri bu haftada başka bir maçta yer alıyor.");

            // Geçerli maç durumları.
            var validStatuses = new[]
            {
                "NotStarted",
                "Live",
                "Finished",
                "Postponed"
            };

            if (!validStatuses.Contains(dto.Status))
                return BadRequest("Geçersiz maç durumu.");

            // Değişiklikleri uygula.
            match.HomeTeamId = dto.HomeTeamId;
            match.AwayTeamId = dto.AwayTeamId;
            match.Week = dto.Week;
            match.MatchDate = dto.MatchDate;
            match.Stadium = dto.Stadium.Trim();
            match.Status = dto.Status;
            // Skorlar elle düzenlenmez; gol kayıtlarından hesaplanır.
            // Maç düzenleme işlemi mevcut skoru burada değiştirmez.

            await _context.SaveChangesAsync();

            return NoContent();
        }




        // DELETE: api/matches/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            var match = await _context.Matches
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
                return NotFound("Maç bulunamadı.");

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpPost("{id:int}/goals")]
        public async Task<IActionResult> AddGoal(
            int id,
            [FromBody] CreateMatchGoalDto dto)
        {
            var match = await _context.Matches
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
                return NotFound("Maç bulunamadı.");

            if (dto == null)
                return BadRequest("Gol bilgileri gönderilmedi.");

            if (dto.TeamId != match.HomeTeamId &&
                dto.TeamId != match.AwayTeamId)
            {
                return BadRequest(
                    "Gol atan takım bu maçın takımlarından biri olmalıdır.");
            }

            if (string.IsNullOrWhiteSpace(dto.PlayerName))
                return BadRequest("Gol atan oyuncunun adı zorunludur.");

            if (dto.PlayerName.Trim().Length > 100)
                return BadRequest("Oyuncu adı en fazla 100 karakter olabilir.");

            if (dto.Minute < 1 || dto.Minute > 130)
                return BadRequest("Gol dakikası 1 ile 130 arasında olmalıdır.");

            var goal = new MatchGoal
            {
                MatchId = id,
                TeamId = dto.TeamId,
                PlayerName = dto.PlayerName.Trim(),
                Minute = dto.Minute
            };

            _context.MatchGoals.Add(goal);
            await _context.SaveChangesAsync();

            await RecalculateScoreAsync(match);

            return Ok(goal);
        }


        // DELETE: api/matches/1/goals/5
        [HttpDelete("{matchId:int}/goals/{goalId:int}")]
        public async Task<IActionResult> DeleteGoal(
            int matchId,
            int goalId)
        {
            var goal = await _context.MatchGoals
                .FirstOrDefaultAsync(g =>
                    g.Id == goalId && g.MatchId == matchId);

            if (goal == null)
                return NotFound("Gol kaydı bulunamadı.");

            var match = await _context.Matches
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
                return NotFound("Maç bulunamadı.");

            _context.MatchGoals.Remove(goal);
            await _context.SaveChangesAsync();

            await RecalculateScoreAsync(match);

            return NoContent();
        }


        // POST: api/matches/1/cards
        [HttpPost("{id:int}/cards")]
        public async Task<IActionResult> AddCard(
            int id,
            [FromBody] CreateMatchCardDto dto)
        {
            var match = await _context.Matches
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
                return NotFound("Maç bulunamadı.");

            // Kart gören takım bu maçta oynamalı.
            if (dto.TeamId != match.HomeTeamId &&
                dto.TeamId != match.AwayTeamId)
            {
                return BadRequest(
                    "Kart gören takım bu maçın takımlarından biri olmalıdır.");
            }

            var card = new MatchCard
            {
                MatchId = id,
                TeamId = dto.TeamId,
                PlayerName = dto.PlayerName.Trim(),
                Minute = dto.Minute,
                CardType = dto.CardType
            };

            _context.MatchCards.Add(card);
            await _context.SaveChangesAsync();

            return Ok(card);
        }


        // DELETE: api/matches/1/cards/5
        [HttpDelete("{matchId:int}/cards/{cardId:int}")]
        public async Task<IActionResult> DeleteCard(
            int matchId,
            int cardId)
        {
            var card = await _context.MatchCards
                .FirstOrDefaultAsync(c =>
                    c.Id == cardId && c.MatchId == matchId);

            if (card == null)
                return NotFound("Kart kaydı bulunamadı.");

            _context.MatchCards.Remove(card);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // POST: api/matches/1/substitutions
        [HttpPost("{id:int}/substitutions")]
        public async Task<IActionResult> AddSubstitution(
            int id,
            [FromBody] CreateSubstitutionDto dto)
        {
            var match = await _context.Matches
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
                return NotFound("Maç bulunamadı.");

            // Oyuncu değişikliği yapan takım bu maçta oynamalı.
            if (dto.TeamId != match.HomeTeamId &&
                dto.TeamId != match.AwayTeamId)
            {
                return BadRequest(
                    "Oyuncu değişikliği yapan takım bu maçın takımlarından biri olmalıdır.");
            }

            var substitution = new Substitution
            {
                MatchId = id,
                TeamId = dto.TeamId,
                PlayerIn = dto.PlayerIn.Trim(),
                PlayerOut = dto.PlayerOut.Trim(),
                Minute = dto.Minute
            };

            _context.Substitutions.Add(substitution);
            await _context.SaveChangesAsync();

            return Ok(substitution);
        }


        // DELETE: api/matches/1/substitutions/5
        [HttpDelete("{matchId:int}/substitutions/{substitutionId:int}")]
        public async Task<IActionResult> DeleteSubstitution(
            int matchId,
            int substitutionId)
        {
            var substitution = await _context.Substitutions
                .FirstOrDefaultAsync(s =>
                    s.Id == substitutionId &&
                    s.MatchId == matchId);

            if (substitution == null)
                return NotFound("Oyuncu değişikliği kaydı bulunamadı.");

            _context.Substitutions.Remove(substitution);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        // Skorun tek kaynağı gol kayıtlarıdır.
        private async Task RecalculateScoreAsync(Match match)
        {
            var homeGoals = await _context.MatchGoals
                .CountAsync(g => g.MatchId == match.Id &&
                                 g.TeamId == match.HomeTeamId);

            var awayGoals = await _context.MatchGoals
                .CountAsync(g => g.MatchId == match.Id &&
                                 g.TeamId == match.AwayTeamId);

            match.HomeScore = homeGoals;
            match.AwayScore = awayGoals;

            await _context.SaveChangesAsync();
        }

    }
}
