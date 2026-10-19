namespace SerieA.Mobile.Models;

public class StandingDto
{
    public int Position { get; set; }
    public int TeamId { get; set; }
    public string TeamName { get; set; } = "";
    public string LogoUrl { get; set; } = "";

    public int Played { get; set; }
    public int Won { get; set; }
    public int Drawn { get; set; }
    public int Lost { get; set; }

    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference { get; set; }
    public int Points { get; set; }

    public string GoalDifferenceText =>
        GoalDifference > 0 ? $"+{GoalDifference}" : GoalDifference.ToString();

    public string LogoImageUrl =>
        string.IsNullOrWhiteSpace(LogoUrl)
            ? ""
            : Uri.TryCreate(LogoUrl, UriKind.Absolute, out _)
                ? LogoUrl
                : $"http://localhost:5046/{LogoUrl.TrimStart('/')}";
}