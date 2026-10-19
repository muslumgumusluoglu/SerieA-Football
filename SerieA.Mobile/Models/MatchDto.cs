namespace SerieA.Mobile.Models;

public class MatchDto
{
    public int Id { get; set; }
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public int Week { get; set; }
    public DateTime MatchDate { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public string Status { get; set; } = "";
    public string Stadium { get; set; } = "";

    public TeamDto? HomeTeam { get; set; }
    public TeamDto? AwayTeam { get; set; }

    public string MatchDateText =>
        MatchDate.ToString("dd.MM.yyyy HH:mm");

    public string ScoreText =>
        HomeScore.HasValue && AwayScore.HasValue
            ? $"{HomeScore} - {AwayScore}"
            : "- : -";

    public string StatusText => Status switch
    {
        "Finished" => "Tamamlandı",
        "Live" => "Canlı",
        "Scheduled" => "Başlamadı",
        _ => Status
    };

    public string HomeLogoUrl => GetLogoUrl(HomeTeam?.LogoUrl);
    public string AwayLogoUrl => GetLogoUrl(AwayTeam?.LogoUrl);

    private static string GetLogoUrl(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return "";

        if (Uri.TryCreate(path, UriKind.Absolute, out _))
            return path;

        return $"http://10.0.2.2:5046/{path.TrimStart('/')}";
    }
}

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string LogoUrl { get; set; } = "";
    public string City { get; set; } = "";
    public string Stadium { get; set; } = "";
}