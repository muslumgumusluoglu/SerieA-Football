using System.Collections.ObjectModel;
using SerieA.Mobile.Models;
using SerieA.Mobile.Services;

namespace SerieA.Mobile;

public partial class MainPage : ContentPage
{
    private readonly ApiService _apiService = new();
    private bool _isLoading;
    private bool _hasLoaded;

    private readonly ObservableCollection<MatchDto> _matches = new();

    public MainPage()
    {
        InitializeComponent();

        MatchesView.ItemsSource = _matches;

        for (int week = 1; week <= 38; week++)
            WeekPicker.Items.Add($"{week}. Hafta");

        WeekPicker.SelectedIndex = 0;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_hasLoaded)
        {
            _hasLoaded = true;
            await LoadMatchesAsync(1);
        }
    }

    private async void OnWeekChanged(object? sender, EventArgs e)
    {
        if (WeekPicker.SelectedIndex < 0)
            return;

        int week = WeekPicker.SelectedIndex + 1;
        await LoadMatchesAsync(week);
    }

    private async Task LoadMatchesAsync(int week)
    {
        if (_isLoading)
            return;

        _isLoading = true;
        MessageLabel.Text = $"{week}. hafta maçları yükleniyor...";

        try
        {
            var matches = await _apiService.GetMatchesByWeekAsync(week);

            _matches.Clear();

            foreach (var match in matches.OrderBy(m => m.MatchDate))
                _matches.Add(match);

            MessageLabel.Text = _matches.Count > 0
                ? $"{week}. hafta · {_matches.Count} maç"
                : $"{week}. hafta için maç bulunamadı.";
        }
        catch (Exception ex)
        {
            MessageLabel.Text =
                "Maçlar yüklenemedi. API bağlantısını kontrol et.";

            await DisplayAlert(
                "Bağlantı hatası",
                $"API'ye ulaşılamadı.\n\n{ex.Message}",
                "Tamam");
        }
        finally
        {
            _isLoading = false;
        }
    }
}