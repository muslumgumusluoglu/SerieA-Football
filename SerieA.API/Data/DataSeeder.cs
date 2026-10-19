
using Microsoft.EntityFrameworkCore;
using SerieA.API.Context;
using SerieA.API.Entities;

namespace SerieA.API.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Takımlar daha önce eklenmişse yeniden ekleme.
            if (!await context.Teams.AnyAsync())
            {
                var teams = new List<Team>
                {
                    new Team { Name = "Inter", City = "Milano", Stadium = "San Siro" },
                    new Team { Name = "AC Milan", City = "Milano", Stadium = "San Siro" },
                    new Team { Name = "Juventus", City = "Torino", Stadium = "Allianz Stadium" },
                    new Team { Name = "Napoli", City = "Napoli", Stadium = "Diego Armando Maradona" },
                    new Team { Name = "AS Roma", City = "Roma", Stadium = "Stadio Olimpico" },
                    new Team { Name = "Lazio", City = "Roma", Stadium = "Stadio Olimpico" },
                    new Team { Name = "Atalanta", City = "Bergamo", Stadium = "Gewiss Stadium" },
                    new Team { Name = "Fiorentina", City = "Firenze", Stadium = "Stadio Artemio Franchi" },
                    new Team { Name = "Bologna", City = "Bologna", Stadium = "Stadio Renato Dall'Ara" },
                    new Team { Name = "Torino", City = "Torino", Stadium = "Stadio Olimpico Grande Torino" },
                    new Team { Name = "Udinese", City = "Udine", Stadium = "Bluenergy Stadium" },
                    new Team { Name = "Genoa", City = "Genova", Stadium = "Stadio Luigi Ferraris" },
                    new Team { Name = "Cagliari", City = "Cagliari", Stadium = "Unipol Domus" },
                    new Team { Name = "Lecce", City = "Lecce", Stadium = "Stadio Via del Mare" },
                    new Team { Name = "Parma", City = "Parma", Stadium = "Stadio Ennio Tardini" },
                    new Team { Name = "Como", City = "Como", Stadium = "Stadio Giuseppe Sinigaglia" },
                    new Team { Name = "Sassuolo", City = "Sassuolo", Stadium = "Mapei Stadium" },
                    new Team { Name = "Hellas Verona", City = "Verona", Stadium = "Stadio Marcantonio Bentegodi" },
                    new Team { Name = "Empoli", City = "Empoli", Stadium = "Stadio Carlo Castellani" },
                    new Team { Name = "Monza", City = "Monza", Stadium = "U-Power Stadium" }
                };

                await context.Teams.AddRangeAsync(teams);
                await context.SaveChangesAsync();
            }

            // Fikstür daha önce oluşturulmuşsa tekrar oluşturma.
            if (await context.Matches.AnyAsync())
                return;

            var teamsInDb = await context.Teams
                .AsNoTracking()
                .OrderBy(t => t.Id)
                .ToListAsync();

            if (teamsInDb.Count != 20)
                throw new InvalidOperationException(
                    "Fikstür oluşturmak için veritabanında tam 20 takım olmalıdır.");

            // Her hafta 10 maç: aynı hafta hiçbir takım tekrarlanmaz.
            int[][] fixtures =
            {
                // 1. hafta
                new[] { 1, 20 }, new[] { 2, 19 },
                new[] { 3, 18 }, new[] { 4, 17 },
                new[] { 5, 16 }, new[] { 6, 15 },
                new[] { 7, 14 }, new[] { 8, 13 },
                new[] { 9, 12 }, new[] { 10, 11 },

                // 2. hafta
                new[] { 1, 19 }, new[] { 20, 18 },
                new[] { 2, 17 }, new[] { 3, 16 },
                new[] { 4, 15 }, new[] { 5, 14 },
                new[] { 6, 13 }, new[] { 7, 12 },
                new[] { 8, 11 }, new[] { 9, 10 },

                // 3. hafta
                new[] { 1, 18 }, new[] { 19, 17 },
                new[] { 20, 16 }, new[] { 2, 15 },
                new[] { 3, 14 }, new[] { 4, 13 },
                new[] { 5, 12 }, new[] { 6, 11 },
                new[] { 7, 10 }, new[] { 8, 9 }
            };

            var matches = new List<Match>();

            for (int i = 0; i < fixtures.Length; i++)
            {
                int week = (i / 10) + 1;
                int matchInWeek = i % 10;

                var match = new Match
                {
                    HomeTeamId = teamsInDb[fixtures[i][0] - 1].Id,
                    AwayTeamId = teamsInDb[fixtures[i][1] - 1].Id,
                    Week = week,
                    MatchDate = new DateTime(2026, 11, 1, 20, 0, 0)
                        .AddDays((week - 1) * 7 + matchInWeek),
                    Stadium = teamsInDb[fixtures[i][0] - 1].Stadium,
                    Status = "NotStarted"
                };

                // İlk üç maçı tamamlanmış örnek maç yapıyoruz.
                if (i == 0)
                {
                    match.Status = "Finished";
                    match.HomeScore = 2;
                    match.AwayScore = 1;
                }
                else if (i == 1)
                {
                    match.Status = "Finished";
                    match.HomeScore = 1;
                    match.AwayScore = 0;
                }
                else if (i == 2)
                {
                    match.Status = "Finished";
                    match.HomeScore = 1;
                    match.AwayScore = 1;
                }

                matches.Add(match);
            }

            await context.Matches.AddRangeAsync(matches);
            await context.SaveChangesAsync();

            var detailedMatches = matches.Take(3).ToList();

            var goals = new List<MatchGoal>
            {
                new MatchGoal
                {
                    MatchId = detailedMatches[0].Id,
                    TeamId = detailedMatches[0].HomeTeamId,
                    PlayerName = "Lautaro Martinez",
                    Minute = 24
                },
                new MatchGoal
                {
                    MatchId = detailedMatches[0].Id,
                    TeamId = detailedMatches[0].AwayTeamId,
                    PlayerName = "Monza Oyuncusu",
                    Minute = 51
                },
                new MatchGoal
                {
                    MatchId = detailedMatches[0].Id,
                    TeamId = detailedMatches[0].HomeTeamId,
                    PlayerName = "Inter Oyuncusu",
                    Minute = 78
                },
                new MatchGoal
                {
                    MatchId = detailedMatches[1].Id,
                    TeamId = detailedMatches[1].HomeTeamId,
                    PlayerName = "Milan Oyuncusu",
                    Minute = 36
                },
                new MatchGoal
                {
                    MatchId = detailedMatches[2].Id,
                    TeamId = detailedMatches[2].HomeTeamId,
                    PlayerName = "Juventus Oyuncusu",
                    Minute = 42
                },
                new MatchGoal
                {
                    MatchId = detailedMatches[2].Id,
                    TeamId = detailedMatches[2].AwayTeamId,
                    PlayerName = "Verona Oyuncusu",
                    Minute = 67
                }
            };

            var cards = new List<MatchCard>
            {
                new MatchCard
                {
                    MatchId = detailedMatches[0].Id,
                    TeamId = detailedMatches[0].AwayTeamId,
                    PlayerName = "Monza Oyuncusu",
                    Minute = 60,
                    CardType = "Yellow"
                },
                new MatchCard
                {
                    MatchId = detailedMatches[1].Id,
                    TeamId = detailedMatches[1].HomeTeamId,
                    PlayerName = "Milan Oyuncusu",
                    Minute = 70,
                    CardType = "Yellow"
                },
                new MatchCard
                {
                    MatchId = detailedMatches[2].Id,
                    TeamId = detailedMatches[2].AwayTeamId,
                    PlayerName = "Verona Oyuncusu",
                    Minute = 75,
                    CardType = "Red"
                }
            };

            var substitutions = new List<Substitution>
            {
                new Substitution
                {
                    MatchId = detailedMatches[0].Id,
                    TeamId = detailedMatches[0].HomeTeamId,
                    PlayerOut = "Inter Oyuncusu A",
                    PlayerIn = "Inter Oyuncusu B",
                    Minute = 70
                },
                new Substitution
                {
                    MatchId = detailedMatches[1].Id,
                    TeamId = detailedMatches[1].HomeTeamId,
                    PlayerOut = "Milan Oyuncusu A",
                    PlayerIn = "Milan Oyuncusu B",
                    Minute = 80
                },
                new Substitution
                {
                    MatchId = detailedMatches[2].Id,
                    TeamId = detailedMatches[2].AwayTeamId,
                    PlayerOut = "Verona Oyuncusu A",
                    PlayerIn = "Verona Oyuncusu B",
                    Minute = 65
                }
            };

            await context.MatchGoals.AddRangeAsync(goals);
            await context.MatchCards.AddRangeAsync(cards);
            await context.Substitutions.AddRangeAsync(substitutions);

            await context.SaveChangesAsync();
        }
    }
}
