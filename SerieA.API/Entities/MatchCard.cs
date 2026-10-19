
namespace SerieA.API.Entities
{
    public class MatchCard
    {
        public int Id { get; set; }

        public int MatchId { get; set; }

        public int TeamId { get; set; }

        public string PlayerName { get; set; } = string.Empty;

        public int Minute { get; set; }

        public string CardType { get; set; } = string.Empty;

        public Match? Match { get; set; }

        public Team? Team { get; set; }
    }
}
