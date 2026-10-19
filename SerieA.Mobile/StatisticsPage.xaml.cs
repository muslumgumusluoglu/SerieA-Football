using System.Text.Json;
using SerieA.Mobile.Services;

namespace SerieA.Mobile;

public partial class StatisticsPage : ContentPage
{
    private readonly ApiService _apiService = new();
    private bool _isLoading;

    public StatisticsPage()
    {
        InitializeComponent();
        Loaded += async (_, _) => await LoadStatisticsAsync();
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadStatisticsAsync();
    }

    private async Task LoadStatisticsAsync()
    {
        if (_isLoading)
            return;

        _isLoading = true;
        RefreshButton.IsEnabled = false;
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        MessageLabel.IsVisible = false;

        try
        {
            await LoadSectionAsync(
                OverviewContainer,
                () => _apiService.GetStatisticsOverviewAsync(),
                "Genel istatistikler alýnamadý.");

            await LoadSectionAsync(
                ScorersContainer,
                () => _apiService.GetTopScorersAsync(),
                "Gol krallýðý verileri alýnamadý.");

            await LoadSectionAsync(
                MatchesContainer,
                () => _apiService.GetHighScoringMatchesAsync(),
                "En gollü maçlar alýnamadý.");

            await LoadSectionAsync(
                GoalsByWeekContainer,
                () => _apiService.GetGoalsByWeekAsync(),
                "Haftalýk gol verileri alýnamadý.");
        }
        catch (Exception ex)
        {
            MessageLabel.Text =
                $"Ýstatistikler yüklenirken hata oluþtu: {ex.Message}";
            MessageLabel.IsVisible = true;
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            RefreshButton.IsEnabled = true;
            _isLoading = false;
        }
    }

    private async Task LoadSectionAsync(
        VerticalStackLayout container,
        Func<Task<JsonElement>> request,
        string errorMessage)
    {
        container.Children.Clear();

        try
        {
            var data = await request();
            RenderJson(container, data);

            if (container.Children.Count == 0)
                AddText(container, "Gösterilecek veri bulunamadý.", "#91A9D2");
        }
        catch
        {
            AddText(container, errorMessage, "#FF7798");
        }
    }

    private void RenderJson(
        VerticalStackLayout container,
        JsonElement data)
    {
        if (data.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in data.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.Object)
                {
                    var nested = new VerticalStackLayout
                    {
                        Spacing = 6,
                        Margin = new Thickness(0, 5, 0, 5)
                    };

                    AddText(
                        nested,
                        TranslateName(property.Name),
                        "#65A0FF",
                        true);

                    RenderJson(nested, property.Value);
                    container.Children.Add(nested);
                }
                else if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    var nested = new VerticalStackLayout
                    {
                        Spacing = 8,
                        Margin = new Thickness(0, 5, 0, 5)
                    };

                    AddText(
                        nested,
                        TranslateName(property.Name),
                        "#65A0FF",
                        true);

                    RenderJson(nested, property.Value);
                    container.Children.Add(nested);
                }
                else
                {
                    AddPropertyRow(
                        container,
                        TranslateName(property.Name),
                        TranslateValue(property.Name, property.Value));
                }
            }

            return;
        }

        if (data.ValueKind == JsonValueKind.Array)
        {
            var index = 0;

            foreach (var item in data.EnumerateArray())
            {
                index++;

                var card = new Border
                {
                    BackgroundColor = Color.FromArgb("#0B1830"),
                    Stroke = Color.FromArgb("#29446D"),
                    StrokeThickness = 1,
                    Padding = new Thickness(10, 9),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                    {
                        CornerRadius = 10
                    }
                };

                var content = new VerticalStackLayout
                {
                    Spacing = 5
                };

                AddText(content, $"{index}.", "#65A0FF", true);
                RenderJson(content, item);

                card.Content = content;
                container.Children.Add(card);
            }

            return;
        }

        if (data.ValueKind is JsonValueKind.String
            or JsonValueKind.Number
            or JsonValueKind.True
            or JsonValueKind.False)
        {
            AddText(container, data.ToString(), "#F2F5FF");
        }
    }

    private void AddPropertyRow(
        VerticalStackLayout container,
        string name,
        string value)
    {
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                },
                new ColumnDefinition { Width = GridLength.Auto }
            },
            ColumnSpacing = 10,
            Padding = new Thickness(0, 7)
        };

        grid.Add(new Label
        {
            Text = name,
            FontSize = 12,
            TextColor = Color.FromArgb("#91A9D2"),
            VerticalTextAlignment = TextAlignment.Center
        }, 0, 0);

        grid.Add(new Label
        {
            Text = value,
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#F2F5FF"),
            HorizontalTextAlignment = TextAlignment.End,
            VerticalTextAlignment = TextAlignment.Center
        }, 1, 0);

        container.Children.Add(grid);

        container.Children.Add(new BoxView
        {
            HeightRequest = 1,
            Color = Color.FromArgb("#243958")
        });
    }

    private void AddText(
        VerticalStackLayout container,
        string text,
        string color,
        bool bold = false)
    {
        container.Children.Add(new Label
        {
            Text = text,
            FontSize = 12,
            TextColor = Color.FromArgb(color),
            FontAttributes = bold
                ? FontAttributes.Bold
                : FontAttributes.None,
            LineBreakMode = LineBreakMode.WordWrap
        });
    }

    private static string TranslateName(string name)
    {
        return name.ToLowerInvariant() switch
        {
            "id" => "Numara",
            "position" => "Sýralama",
            "rank" => "Sýralama",

            "player" => "Oyuncu",
            "playername" => "Oyuncu",
            "scorername" => "Oyuncu",
            "name" => "Ad",

            "team" => "Takým",
            "teamname" => "Takým",
            "teamid" => "Takým numarasý",
            "teamlogo" => "Takým logosu",
            "logourl" => "Takým logosu",

            "goals" => "Gol",
            "goal" => "Gol",
            "totalgoals" => "Toplam gol",
            "goalsfor" => "Atýlan gol",
            "goalsagainst" => "Yenilen gol",
            "goaldifference" => "Averaj",

            "played" => "Oynanan maç",
            "matchesplayed" => "Oynanan maç",
            "won" => "Galibiyet",
            "wins" => "Galibiyet",
            "drawn" => "Beraberlik",
            "draws" => "Beraberlik",
            "lost" => "Maðlubiyet",
            "losses" => "Maðlubiyet",
            "points" => "Puan",

            "assists" => "Asist",
            "appearances" => "Oynadýðý maç",

            "matchdate" => "Maç tarihi",
            "date" => "Tarih",
            "week" => "Hafta",
            "weeknumber" => "Hafta",

            "hometeam" => "Ev sahibi",
            "hometeamname" => "Ev sahibi",
            "awayteam" => "Deplasman",
            "awayteamname" => "Deplasman",
            "homescore" => "Ev sahibi golü",
            "awayscore" => "Deplasman golü",

            "stadium" => "Stadyum",
            "status" => "Maç durumu",

            "totalteams" => "Takým sayýsý",
            "totalmatches" => "Toplam maç",
            "finishedmatches" => "Tamamlanan maç",
            "remainingmatches" => "Kalan maç",
            "totalgoalsleague" => "Toplam gol",
            "averagegoals" => "Maç baþýna gol",
            "goalspermatch" => "Maç baþýna gol",
            "completedweeks" => "Tamamlanan hafta",
            "totalcards" => "Toplam kart",

            _ => SplitName(name)
        };
    }

    private static string SplitName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return name;

        var result = new System.Text.StringBuilder();

        for (int i = 0; i < name.Length; i++)
        {
            if (i > 0 &&
                char.IsUpper(name[i]) &&
                (char.IsLower(name[i - 1]) ||
                 char.IsDigit(name[i - 1])))
            {
                result.Append(' ');
            }

            result.Append(name[i]);
        }

        return result.ToString();
    }

    private static string TranslateValue(string propertyName, JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.Null)
            return "-";

        if (value.ValueKind == JsonValueKind.True)
            return "Evet";

        if (value.ValueKind == JsonValueKind.False)
            return "Hayýr";

        if (value.ValueKind == JsonValueKind.String)
        {
            var text = value.GetString() ?? "";

            if (propertyName.Equals("status",
                StringComparison.OrdinalIgnoreCase))
            {
                return text.ToLowerInvariant() switch
                {
                    "finished" => "Tamamlandý",
                    "scheduled" => "Planlandý",
                    "live" => "Canlý",
                    "postponed" => "Ertelendi",
                    "cancelled" => "Ýptal edildi",
                    _ => text
                };
            }

            return text;
        }

        return value.ToString();
    }
}