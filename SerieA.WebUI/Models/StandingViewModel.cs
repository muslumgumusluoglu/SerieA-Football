namespace SerieA.WebUI.Models
{
    public class StandingViewModel
    {
        public int Position { get; set; }

        public int TeamId { get; set; }

        public string TeamName { get; set; } = string.Empty;

        public string? LogoUrl { get; set; }

        public int Played { get; set; }

        public int Won { get; set; }

        public int Drawn { get; set; }

        public int Lost { get; set; }

        public int GoalsFor { get; set; }

        public int GoalsAgainst { get; set; }

        public int GoalDifference => GoalsFor - GoalsAgainst;

        public int Points { get; set; }
    }
}
