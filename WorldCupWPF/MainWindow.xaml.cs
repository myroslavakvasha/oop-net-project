using DataLayer.Models;
using DataLayer.Services;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WorldCupWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ConfigService _configService;
        private readonly SelectionService _selectionService;
        private readonly SettingsService _settingsService;
        private DataService _dataService;

        private bool _isInitialLoading = true;

        public MainWindow(ConfigService configService, SelectionService selectionService, SettingsService settingsService)
        {
            InitializeComponent();

            _configService = configService;
            _selectionService = selectionService;
            _settingsService = settingsService;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Safety double check handled in App.xaml.cs, but ensures memory structures are loaded
            _selectionService.Load();

            try
            {
                loadingOverlay.Visibility = Visibility.Visible;

                _dataService = new DataService(_configService, _selectionService);
                System.Diagnostics.Debug.WriteLine($"Active Championship: {_selectionService.Gender}");

                // 1. Fetch all teams for the active championship
                var teams = await _dataService.GetTeamsAsync();
                var orderedTeams = teams.OrderBy(t => t.Country).ToList();

                // 2. Bind to the favorite/home team ComboBox
                cbFavTeam.ItemsSource = orderedTeams;

                // 3. AUTOMATICALLY PRE-SELECT: Find the team chosen in WinForms
                if (!string.IsNullOrEmpty(_selectionService.Team))
                {
                    var savedTeam = orderedTeams.FirstOrDefault(t => t.FifaCode == _selectionService.Team);
                    if (savedTeam != null)
                    {
                        cbFavTeam.SelectedItem = savedTeam;

                        // Load opponents right away for this pre-selected team
                        await LoadOpponentsAsync(savedTeam.FifaCode);
                    }
                }

                _isInitialLoading = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading initial match data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                loadingOverlay.Visibility = Visibility.Collapsed;
            }
        }

        private async void CbFavTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Avoid triggering during the initial setup bindings
            if (_isInitialLoading) return;

            if (cbFavTeam.SelectedItem is Team selectedTeam)
            {
                loadingOverlay.Visibility = Visibility.Visible;
                try
                {
                    // Save the new primary team selection down to the shared file
                    _selectionService.Save(_selectionService.Gender, selectedTeam.FifaCode, _selectionService.FavoritesByTeam);

                    // Update opponents list dynamically
                    await LoadOpponentsAsync(selectedTeam.FifaCode);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error switching teams: {ex.Message}");
                }
                finally
                {
                    loadingOverlay.Visibility = Visibility.Collapsed;
                }
            }
        }

        private async Task LoadOpponentsAsync(string teamFifaCode)
        {
            // Get all matches for this specific team to find out who they played against
            var matches = await _dataService.GetMatchesByTeamAsync(teamFifaCode);

            var opponentTeams = new List<Team>();

            foreach (var match in matches)
            {
                // If our team was Home, the opponent is Away (and vice versa)
                var opponent = match.HomeTeam.Code == teamFifaCode ? match.AwayTeam : match.HomeTeam;

                opponentTeams.Add(new Team
                {
                    Country = opponent.Country,
                    FifaCode = opponent.Code
                });
            }

            // Bind the matching opponents dropdown
            cbOpponentTeam.ItemsSource = opponentTeams.OrderBy(o => o.Country).ToList();

            if (opponentTeams.Any())
            {
                cbOpponentTeam.SelectedIndex = 0; // Default select the first opponent match frame
            }
        }

        private void CbOpponentTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbOpponentTeam.SelectedItem is Team selectedOpponent)
            {
                // TODO: This is where we trigger loading match statistics 
                // and drawing players onto the field graphics container!
                System.Diagnostics.Debug.WriteLine($"Selected Opponent Match context: {selectedOpponent.Country}");
            }
        }
    }
}