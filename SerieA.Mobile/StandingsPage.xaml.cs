using SerieA.Mobile.Models;
using SerieA.Mobile.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SerieA.Mobile;

public partial class StandingsPage : ContentPage
{
    private readonly ApiService _apiService = new();
    private readonly ObservableCollection<StandingRow> _rows = new();

    private List<StandingRow> _allRows = new();
    private bool _isLoading;
    private string _activeFilter = "all";

    public StandingsPage()
    {
        InitializeComponent();
        StandingsView.ItemsSource = _rows;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_allRows.Count == 0)
            await LoadStandingsAsync();
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadStandingsAsync();
    }

    private async Task LoadStandingsAsync()
    {
        if (_isLoading)
            return;

        _isLoading = true;
        MessageLabel.Text = "Puan durumu yükleniyor...";

        try
        {
            var data = await _apiService.GetStandingsAsync();

            _allRows = data
                .OrderBy(x => x.Position)
                .Select(x => new StandingRow(x))
                .ToList();

            ApplyFilters();
            MessageLabel.Text = $"{_rows.Count} takým gösteriliyor.";
        }
        catch (Exception ex)
        {
            MessageLabel.Text = "Puan durumu yüklenemedi.";

            await DisplayAlert(
                "Baðlantý hatasý",
                ex.Message,
                "Tamam");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFilters();
    }

    private void OnAllFilterClicked(object sender, EventArgs e)
    {
        _activeFilter = "all";
        UpdateFilterButtons();
        ApplyFilters();
    }

    private void OnChampionsFilterClicked(object sender, EventArgs e)
    {
        _activeFilter = "champions";
        UpdateFilterButtons();
        ApplyFilters();
    }

    private void OnEuropeFilterClicked(object sender, EventArgs e)
    {
        _activeFilter = "europe";
        UpdateFilterButtons();
        ApplyFilters();
    }

    private void OnRelegationFilterClicked(object sender, EventArgs e)
    {
        _activeFilter = "relegation";
        UpdateFilterButtons();
        ApplyFilters();
    }

    private void UpdateFilterButtons()
    {
        var buttons = new[]
        {
            (Button: AllFilterButton, Key: "all"),
            (Button: ChampionsFilterButton, Key: "champions"),
            (Button: EuropeFilterButton, Key: "europe"),
            (Button: RelegationFilterButton, Key: "relegation")
        };

        foreach (var item in buttons)
        {
            bool active = item.Key == _activeFilter;
            item.Button.BackgroundColor = active
                ? Color.FromArgb("#183867")
                : Color.FromArgb("#0D1A31");

            item.Button.TextColor = active
                ? Colors.White
                : Color.FromArgb("#A9BEDF");
        }
    }

    private void ApplyFilters()
    {
        string search = SearchEntry?.Text?.Trim() ?? "";

        var filtered = _allRows.Where(row =>
        {
            bool matchesSearch = string.IsNullOrWhiteSpace(search)
                || row.TeamName.Contains(search, StringComparison.OrdinalIgnoreCase);

            bool matchesFilter = _activeFilter switch
            {
                "champions" => row.Position >= 1 && row.Position <= 4,
                "europe" => row.Position >= 5 && row.Position <= 6,
                "relegation" => row.Position >= 18 && row.Position <= 20,
                _ => true
            };

            return matchesSearch && matchesFilter;
        });

        _rows.Clear();

        foreach (var row in filtered)
            _rows.Add(row);

        if (MessageLabel is not null)
            MessageLabel.Text = $"{_rows.Count} takým gösteriliyor.";
    }

    private sealed class StandingRow : INotifyPropertyChanged
    {
        public int Position { get; }
        public int TeamId { get; }
        public string TeamName { get; }
        public string LogoImageUrl { get; }
        public int Played { get; }
        public int Won { get; }
        public int Drawn { get; }
        public int Lost { get; }
        public int GoalsFor { get; }
        public int GoalsAgainst { get; }
        public int GoalDifference { get; }
        public int Points { get; }

        public string GoalDifferenceText =>
            GoalDifference > 0 ? $"+{GoalDifference}" : GoalDifference.ToString();

        public Color PositionColor => Position switch
        {
            <= 4 => Color.FromArgb("#438BFF"),
            <= 6 => Color.FromArgb("#20D5AA"),
            >= 18 => Color.FromArgb("#FF4778"),
            _ => Color.FromArgb("#101E38")
        };

        public StandingRow(StandingDto item)
        {
            Position = item.Position;
            TeamId = item.TeamId;
            TeamName = item.TeamName;
            LogoImageUrl = item.LogoImageUrl;
            Played = item.Played;
            Won = item.Won;
            Drawn = item.Drawn;
            Lost = item.Lost;
            GoalsFor = item.GoalsFor;
            GoalsAgainst = item.GoalsAgainst;
            GoalDifference = item.GoalDifference;
            Points = item.Points;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
