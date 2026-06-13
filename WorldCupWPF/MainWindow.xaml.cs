using DataLayer.Models;
using DataLayer.Models.Enums;
using DataLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WorldCupWPF
{
    public partial class MainWindow : Window
    {
        private readonly SelectionService _selectionService;
        private readonly SettingsService _settingsService;
        private readonly ConfigService _configService;
        private DataService _dataService;

        private bool _isInitialLoading = true;
        private Match _currentActiveMatch = null;
        private bool _isResizingLayout = false;
        public Match CurrentActiveMatch => _currentActiveMatch;
        public Grid MainWindowDimOverlay => DimOverlay;

        public MainWindow(ConfigService configService, SelectionService selectionService, SettingsService settingsService)
        {
            InitializeComponent();
            _configService = configService;
            _selectionService = selectionService;
            _settingsService = settingsService;

            this.StateChanged += MainWindow_StateChanged;

            pitchCanvas.SizeChanged += (s, e) => {
                if (_currentActiveMatch != null && cbFavTeam.SelectedItem is Team favTeam)
                {
                    _isResizingLayout = true;
                    DrawLineupOnPitch(_currentActiveMatch, favTeam.FifaCode);
                    _isResizingLayout = false;
                }
            };
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                _settingsService.Save(_settingsService.Title, true, _settingsService.Width, _settingsService.Height);
            }
            else if (this.WindowState == WindowState.Normal)
            {
                int restoredWidth = (int)this.ActualWidth;
                int restoredHeight = (int)this.ActualHeight;

                if (restoredWidth > 0 && restoredHeight > 0)
                {
                    _settingsService.Save(_settingsService.Title, false, restoredWidth, restoredHeight);
                }
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _settingsService.Load();
            ApplyApplicationDisplaySettings();
            _selectionService.Load();

            await LoadTournamentDataAsync();
        }

        private async Task LoadTournamentDataAsync()
        {
            try
            {
                loadingOverlay.Visibility = Visibility.Visible;
                _dataService = new DataService(_configService, _selectionService);

                if (!string.IsNullOrEmpty(_selectionService.Gender))
                {
                    if (_selectionService.Gender.Equals("male", StringComparison.OrdinalIgnoreCase))
                        txtChampionshipName.Text = "Male World Cup 2018";
                    else if (_selectionService.Gender.Equals("female", StringComparison.OrdinalIgnoreCase))
                        txtChampionshipName.Text = "Female World Cup 2019";
                }

                var teams = await _dataService.GetTeamsAsync();
                var orderedTeams = teams.OrderBy(t => t.Country).ToList();

                cbFavTeam.ItemsSource = orderedTeams;

                if (!string.IsNullOrEmpty(_selectionService.Team))
                {
                    var savedTeam = orderedTeams.FirstOrDefault(t => t.FifaCode == _selectionService.Team);
                    if (savedTeam != null)
                    {
                        cbFavTeam.SelectedItem = savedTeam;
                        await LoadOpponentsAsync(savedTeam.FifaCode);

                        // RESTORE OPPONENT SELECTION FROM FILE:
                        if (!string.IsNullOrEmpty(_selectionService.OpponentTeam))
                        {
                            var savedOpponent = (cbOpponentTeam.ItemsSource as List<Team>)?
                                .FirstOrDefault(t => t.FifaCode == _selectionService.OpponentTeam);

                            if (savedOpponent != null)
                            {
                                cbOpponentTeam.SelectedItem = savedOpponent;
                            }
                        }
                    }
                }

                _isInitialLoading = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading match data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                loadingOverlay.Visibility = Visibility.Collapsed;
            }
        }

        private async void BtnOpenSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWin = new SettingsWindow(_selectionService, _settingsService);

            if (settingsWin.ShowDialog() == true)
            {
                _settingsService.Load();
                _selectionService.Load();

                ApplyApplicationDisplaySettings();

                cbFavTeam.SelectionChanged -= CbFavTeam_SelectionChanged;
                cbOpponentTeam.SelectionChanged -= CbOpponentTeam_SelectionChanged;

                cbFavTeam.ItemsSource = null;
                cbOpponentTeam.ItemsSource = null;
                cbFavTeam.Items.Clear();
                cbOpponentTeam.Items.Clear();
                txtResult.Text = "- : -";
                pitchCanvas.Children.Clear();

                cbFavTeam.SelectionChanged += CbFavTeam_SelectionChanged;
                cbOpponentTeam.SelectionChanged += CbOpponentTeam_SelectionChanged;

                await LoadTournamentDataAsync();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to exit the application?",
                "Confirm Exit",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.Yes
            );

            if (result != MessageBoxResult.Yes) e.Cancel = true;
        }

        private void ApplyApplicationDisplaySettings()
        {
            this.Title = _settingsService.Title;
            this.WindowStyle = WindowStyle.SingleBorderWindow;

            this.StateChanged -= MainWindow_StateChanged;

            if (_settingsService.IsFullScreen)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
                this.Width = _settingsService.Width;
                this.Height = _settingsService.Height;

                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;

                this.Left = (screenWidth - this.Width) / 2;
                this.Top = (screenHeight - this.Height) / 2;
            }

            this.StateChanged += MainWindow_StateChanged;
        }

        private async void CbFavTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitialLoading || cbFavTeam.SelectedItem == null) return;

            if (e.RemovedItems.Count == 0 && !string.IsNullOrEmpty(_selectionService.OpponentTeam))
            {
                return;
            }

            if (cbFavTeam.SelectedItem is Team selectedTeam)
            {
                string opponentToSave = string.Empty;
                if (_selectionService.Team == selectedTeam.FifaCode)
                {
                    opponentToSave = _selectionService.OpponentTeam;
                }

                _selectionService.Save(_selectionService.Gender, selectedTeam.FifaCode, opponentToSave, _selectionService.FavoritesByTeam);

                await LoadOpponentsAsync(selectedTeam.FifaCode);
            }
        }

        private async Task LoadOpponentsAsync(string favTeamFifaCode)
        {
            if (string.IsNullOrEmpty(favTeamFifaCode)) return;

            try
            {
                var matches = await _dataService.GetMatchesByTeamAsync(favTeamFifaCode);
                var opponentTeams = new List<Team>();

                foreach (var match in matches)
                {
                    var allTeams = cbFavTeam.ItemsSource as List<Team>;
                    string targetCode = match.HomeTeam.Code.Equals(favTeamFifaCode, StringComparison.OrdinalIgnoreCase)
                        ? match.AwayTeam.Code
                        : match.HomeTeam.Code;

                    var opponent = allTeams?.FirstOrDefault(t => t.FifaCode.Equals(targetCode, StringComparison.OrdinalIgnoreCase));
                    if (opponent != null && !opponentTeams.Any(t => t.FifaCode == opponent.FifaCode))
                    {
                        opponentTeams.Add(opponent);
                    }
                }

                cbOpponentTeam.ItemsSource = opponentTeams.OrderBy(t => t.Country).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error mapping opponents: {ex.Message}");
            }
        }

        private async void CbOpponentTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbOpponentTeam.SelectedItem == null)
            {
                if (_currentActiveMatch != null || !string.IsNullOrEmpty(_selectionService.OpponentTeam))
                {
                    return;
                }

                txtResult.Text = "- : -";
                pitchCanvas.Children.Clear();
                return;
            }

            if (cbFavTeam.SelectedItem is Team favTeam && cbOpponentTeam.SelectedItem is Team opponentTeam)
            {
                try
                {
                    _selectionService.Save(_selectionService.Gender, favTeam.FifaCode, opponentTeam.FifaCode, _selectionService.FavoritesByTeam);

                    var matches = await _dataService.GetMatchesByTeamAsync(favTeam.FifaCode);
                    var headToHeadMatch = matches.FirstOrDefault(m =>
                        (m.HomeTeam.Code.Equals(favTeam.FifaCode, StringComparison.OrdinalIgnoreCase) && m.AwayTeam.Code.Equals(opponentTeam.FifaCode, StringComparison.OrdinalIgnoreCase)) ||
                        (m.HomeTeam.Code.Equals(opponentTeam.FifaCode, StringComparison.OrdinalIgnoreCase) && m.AwayTeam.Code.Equals(favTeam.FifaCode, StringComparison.OrdinalIgnoreCase))
                    );

                    if (headToHeadMatch != null)
                    {
                        bool isFavHome = headToHeadMatch.HomeTeam.Code.Equals(favTeam.FifaCode, StringComparison.OrdinalIgnoreCase);
                        long favGoals = isFavHome ? headToHeadMatch.HomeTeam.Goals : headToHeadMatch.AwayTeam.Goals;
                        long oppGoals = isFavHome ? headToHeadMatch.AwayTeam.Goals : headToHeadMatch.HomeTeam.Goals;

                        txtResult.Text = $"{favGoals} : {oppGoals}";

                        _currentActiveMatch = headToHeadMatch;
                        DrawLineupOnPitch(headToHeadMatch, favTeam.FifaCode);
                    }
                    else
                    {
                        txtResult.Text = "- : -";
                        pitchCanvas.Children.Clear();
                        _currentActiveMatch = null;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error retrieving head-to-head score results: {ex.Message}");
                    txtResult.Text = "- : -";
                }
            }
        }

        private void BtnFavTeamInfo_Click(object sender, RoutedEventArgs e)
        {
            if (cbFavTeam.SelectedItem is Team selectedTeam)
            {
                DimOverlay.Visibility = Visibility.Visible;
                TeamInfoWindow infoWin = new TeamInfoWindow(selectedTeam) { Owner = this };
                infoWin.ShowDialog();
                DimOverlay.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Please select a favorite national team first.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnOpponentTeamInfo_Click(object sender, RoutedEventArgs e)
        {
            if (cbOpponentTeam.SelectedItem is Team selectedTeam)
            {
                DimOverlay.Visibility = Visibility.Visible;
                TeamInfoWindow infoWin = new TeamInfoWindow(selectedTeam) { Owner = this };
                infoWin.ShowDialog();
                DimOverlay.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Please select an opponent national team first.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DrawLineupOnPitch(Match match, string favoriteTeamCode)
        {
            pitchCanvas.Children.Clear();
            if (pitchCanvas.ActualWidth == 0 || pitchCanvas.ActualHeight == 0) return;

            bool isFavHome = match.HomeTeam.Code.Equals(favoriteTeamCode, StringComparison.OrdinalIgnoreCase);

            var leftTeamStats = isFavHome ? match.HomeTeamStatistics : match.AwayTeamStatistics;
            var leftTeamCode = isFavHome ? match.HomeTeam.Code : match.AwayTeam.Code;

            var rightTeamStats = isFavHome ? match.AwayTeamStatistics : match.HomeTeamStatistics;
            var rightTeamCode = isFavHome ? match.AwayTeam.Code : match.HomeTeam.Code;

            string activeGender = _selectionService.Gender;

            if (leftTeamStats != null)
                RenderTeamHalf(leftTeamStats.StartingEleven, activeGender, leftTeamCode, isLeftHalf: true);

            if (rightTeamStats != null)
                RenderTeamHalf(rightTeamStats.StartingEleven, activeGender, rightTeamCode, isLeftHalf: false);
        }

        private void RenderTeamHalf(List<Player> players, string gender, string countryCode, bool isLeftHalf)
        {
            double pitchWidth = pitchCanvas.ActualWidth;
            double pitchHeight = pitchCanvas.ActualHeight;

            var positionsGroup = players.GroupBy(p => p.Position).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var pair in positionsGroup)
            {
                Position role = pair.Key;
                var rolePlayers = pair.Value;

                double xPercent = 0;
                switch (role)
                {
                    case Position.Goalie: xPercent = isLeftHalf ? 0.08 : 0.92; break;
                    case Position.Defender: xPercent = isLeftHalf ? 0.22 : 0.78; break;
                    case Position.Midfield: xPercent = isLeftHalf ? 0.36 : 0.64; break;
                    case Position.Forward: xPercent = isLeftHalf ? 0.46 : 0.54; break;
                }

                double xPos = (pitchWidth * xPercent) - (85 / 2.0);

                for (int i = 0; i < rolePlayers.Count; i++)
                {
                    double yPercent = (double)(i + 1) / (rolePlayers.Count + 1);
                    double yPos = (pitchHeight * yPercent) - (105 / 2.0);

                    PlayerControl playerBadge = new PlayerControl();
                    playerBadge.PopulatePlayerData(rolePlayers[i], gender, countryCode);

                    Canvas.SetLeft(playerBadge, xPos);
                    Canvas.SetTop(playerBadge, yPos);

                    pitchCanvas.Children.Add(playerBadge);
                }
            }
        }
    }
}