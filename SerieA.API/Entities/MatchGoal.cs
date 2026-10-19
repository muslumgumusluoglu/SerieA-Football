
namespace SerieA.API.Entities
{
    public class MatchGoal
    {
        public int Id { get; set; }

        public int MatchId { get; set; }

        public int TeamId { get; set; }

        public string PlayerName { get; set; } = string.Empty;

        public int Minute { get; set; }

        public Match? Match { get; set; }

        public Team? Team { get; set; }
    }
}
