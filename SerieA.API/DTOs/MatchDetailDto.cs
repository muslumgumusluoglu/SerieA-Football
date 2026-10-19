namespace SerieA.API.DTOs
{
    public class MatchDetailDto
    {
        public int Id { get; set; }

        public int HomeTeamId { get; set; }

        public string HomeTeamName { get; set; } = string.Empty;

        public string? HomeTeamLogoUrl { get; set; }

        public int AwayTeamId { get; set; }

        public string AwayTeamName { get; set; } = string.Empty;

        public string? AwayTeamLogoUrl { get; set; }

        public int Week { get; set; }

        public DateTime MatchDate { get; set; }

        public int? HomeScore { get; set; }

        public int? AwayScore { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Stadium { get; set; } = string.Empty;

        public List<MatchGoalDto> Goals { get; set; } = new();

        public List<MatchCardDto> Cards { get; set; } = new();

        public List<SubstitutionDto> Substitutions { get; set; } = new();
    }

    public class MatchGoalDto
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int Minute { get; set; }
    }

    public class MatchCardDto
    {
        public int Id { get; set; }
        public int TeamId { get; set; }

        public string PlayerName { get; set; } = string.Empty;

        public int Minute { get; set; }

        public string CardType { get; set; } = string.Empty;
    }

    public class SubstitutionDto
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string PlayerIn { get; set; } = string.Empty;
        public string PlayerOut { get; set; } = string.Empty;
        public int Minute { get; set; }
    }
}
