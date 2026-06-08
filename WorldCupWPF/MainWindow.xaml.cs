using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DataLayer.Models;
using DataLayer.Services;

namespace WorldCupWPF
{
    public partial class MainWindow : Window
    {
        private readonly SelectionService _selectionService;
        private readonly SettingsService _settingsService;
        private readonly ConfigService _configService;
        private DataService _dataService;

        private bool _isInitialLoading = true;

        public MainWindow(ConfigService configService, SelectionService selectionService, SettingsService settingsService)
        {
            InitializeComponent();
            _configService = configService;
            _selectionService = selectionService;
            _settingsService = settingsService;

            this.StateChanged += MainWindow_StateChanged;
        }

        // fullscreen handle
        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                _settingsService.Save(_settingsService.Title, true, _settingsService.Width, _settingsService.Height);
            }
            else if (this.WindowState == WindowState.Normal)
            {
                // Capture the standard running restoration size coordinates 
                int restoredWidth = (int)this.ActualWidth;
                int restoredHeight = (int)this.ActualHeight;

                // Fallback safety filter just in case metrics load before layout initialization sets values
                if (restoredWidth > 0 && restoredHeight > 0)
                {
                    _settingsService.Save(_settingsService.Title, false, restoredWidth, restoredHeight);
                }
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 1. Force the settings service to explicitly pull the saved WPF text file parameters
            _settingsService.Load();

            // 2. Instantly apply those specific layout values to the active window wrapper
            ApplyApplicationDisplaySettings();

            // 3. Keep your existing selection structural startup data running
            _selectionService.Load();

            // 4. Populate teams data from the underlying service configurations
            await LoadTournamentDataAsync();
        }

        // --- CORE REUSE STRATEGY FOR REFRESHING DATA ---
        private async Task LoadTournamentDataAsync()
        {
            try
            {
                loadingOverlay.Visibility = Visibility.Visible;

                // Re-instantiate DataService so it binds dynamically to the updated configuration
                _dataService = new DataService(_configService, _selectionService);
                System.Diagnostics.Debug.WriteLine($"Active Championship: {_selectionService.Gender}");

                // DYNAMIC HEADER TEXT UPDATE: Reads current gender data state and formats it cleanly
                if (!string.IsNullOrEmpty(_selectionService.Gender))
                {
                    if (_selectionService.Gender.Equals("male", StringComparison.OrdinalIgnoreCase))
                    {
                        txtChampionshipName.Text = "Male World Cup 2018";
                    }
                    else if (_selectionService.Gender.Equals("female", StringComparison.OrdinalIgnoreCase))
                    {
                        txtChampionshipName.Text = "Female World Cup 2019";
                    }
                }

                // Fetch data cleanly from the backend service layers
                var teams = await _dataService.GetTeamsAsync();
                var orderedTeams = teams.OrderBy(t => t.Country).ToList();

                // Bind elements to UI Dropdowns
                cbFavTeam.ItemsSource = orderedTeams;

                // Handle pre-selection behaviors inherited across applications
                if (!string.IsNullOrEmpty(_selectionService.Team))
                {
                    var savedTeam = orderedTeams.FirstOrDefault(t => t.FifaCode == _selectionService.Team);
                    if (savedTeam != null)
                    {
                        cbFavTeam.SelectedItem = savedTeam;
                        await LoadOpponentsAsync(savedTeam.FifaCode);
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

        // --- SETTINGS DECK WINDOW TRIGGER ---
        private async void BtnOpenSettings_Click(object sender, RoutedEventArgs e)
        {
            // Extract the currently selected team's FIFA code from the ComboBox
            string currentFifaCode = string.Empty;
            if (cbFavTeam.SelectedItem is Team selectedTeam)
            {
                currentFifaCode = selectedTeam.FifaCode;
            }
            else if (!string.IsNullOrEmpty(_selectionService.Team))
            {
                // Fallback if UI hasn't fully registered selection yet
                currentFifaCode = _selectionService.Team;
            }

            // Pass the current team code as the 3rd argument
            SettingsWindow settingsWin = new SettingsWindow(_selectionService, _settingsService);

            if (settingsWin.ShowDialog() == true)
            {
                // Force sync physical text file states into live configuration service layers
                _settingsService.Load();
                _selectionService.Load();

                // Redraw display viewport geometry limits and update task text fields immediately
                ApplyApplicationDisplaySettings();

                // Safely clean structural state objects to prevent calculation crashes during reset
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

                // Fire async asset update task chain to reload current Gender parameters
                await LoadTournamentDataAsync();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Setting the default button to 'Button1' (which is 'Yes') forces 
            // the Enter key to automatically execute the exit sequence.
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to exit the application?",
                "Confirm Exit",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.Yes // <- Focuses 'Yes'. Enter now exits!
            );

            if (result != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }

        // --- HELPER METHOD: ENFORCES VISUALS IMMEDIATELY ---
        // --- HELPER METHOD: ENFORCES VISUALS IMMEDIATELY ---
        // --- HELPER METHOD: ENFORCES VISUALS IMMEDIATELY ---
        private void ApplyApplicationDisplaySettings()
        {
            this.Title = _settingsService.Title;
            this.WindowStyle = WindowStyle.SingleBorderWindow;

            // CRITICAL FIX: Unhook the event listener so state changes don't overwrite our values mid-resize
            this.StateChanged -= MainWindow_StateChanged;

            if (_settingsService.IsFullScreen)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                // 1. Drop out of Maximized state safely
                this.WindowState = WindowState.Normal;

                // 2. Explicitly force the target configuration parameters
                this.Width = _settingsService.Width;
                this.Height = _settingsService.Height;

                // 3. Center the window viewport coordinates on the monitor
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;

                this.Left = (screenWidth - this.Width) / 2;
                this.Top = (screenHeight - this.Height) / 2;
            }

            // CRITICAL FIX: Re-hook the event listener now that sizes are cleanly updated
            this.StateChanged += MainWindow_StateChanged;
        }

        private async void CbFavTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Safety check: skip if layout is dropping its bindings during initialization/reloads
            if (cbFavTeam.SelectedItem == null || _isInitialLoading) return;

            if (cbFavTeam.SelectedItem is Team selectedTeam)
            {
                // JUST LIKE WINFORMS: Save immediately when the team selection drops down
                _selectionService.Save(_selectionService.Gender, selectedTeam.FifaCode, _selectionService.FavoritesByTeam);

                // Load the visual opponents matching this team context
                await LoadOpponentsAsync(selectedTeam.FifaCode);
            }
        }

        private async Task LoadOpponentsAsync(string favTeamFifaCode)
        {
            // Double check that we actually have a valid code to look up
            if (string.IsNullOrEmpty(favTeamFifaCode)) return;

            try
            {
                // 1. Fetch all matches involving the favorite team
                var matches = await _dataService.GetMatchesByTeamAsync(favTeamFifaCode);
                var opponentTeams = new List<Team>();

                // 2. Loop through the matches to find who they played against
                foreach (var match in matches)
                {
                    // Determine if your favorite team was Home or Away, then grab the opposite team
                    if (match.HomeTeam.Code.Equals(favTeamFifaCode, StringComparison.OrdinalIgnoreCase))
                    {
                        // We need to fetch the full Team object matching match.AwayTeam.Code from your master dataset
                        var allTeams = cbFavTeam.ItemsSource as List<Team>;
                        var opponent = allTeams?.FirstOrDefault(t => t.FifaCode.Equals(match.AwayTeam.Code, StringComparison.OrdinalIgnoreCase));
                        if (opponent != null && !opponentTeams.Any(t => t.FifaCode == opponent.FifaCode))
                            opponentTeams.Add(opponent);
                    }
                    else
                    {
                        var allTeams = cbFavTeam.ItemsSource as List<Team>;
                        var opponent = allTeams?.FirstOrDefault(t => t.FifaCode.Equals(match.HomeTeam.Code, StringComparison.OrdinalIgnoreCase));
                        if (opponent != null && !opponentTeams.Any(t => t.FifaCode == opponent.FifaCode))
                            opponentTeams.Add(opponent);
                    }
                }

                // 3. Bind the clean opponents list back to the UI dropdown
                cbOpponentTeam.ItemsSource = opponentTeams.OrderBy(t => t.Country).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error mapping opponents: {ex.Message}");
            }
        }

        private async void CbOpponentTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // SAFETY CIRCUIT BREAKER: Stop executing if selections are wiped out during a settings reload
            if (cbFavTeam.SelectedItem == null || cbOpponentTeam.SelectedItem == null)
            {
                txtResult.Text = "- : -";
                pitchCanvas.Children.Clear();
                return;
            }

            if (cbFavTeam.SelectedItem is Team favTeam && cbOpponentTeam.SelectedItem is Team opponentTeam)
            {
                try
                {
                    // 1. Fetch the match histories involving the favorite team
                    var matches = await _dataService.GetMatchesByTeamAsync(favTeam.FifaCode);

                    // 2. Locate the specific match played against this opponent
                    var headToHeadMatch = matches.FirstOrDefault(m =>
                        (m.HomeTeam.Code.Equals(favTeam.FifaCode, StringComparison.OrdinalIgnoreCase) && m.AwayTeam.Code.Equals(opponentTeam.FifaCode, StringComparison.OrdinalIgnoreCase)) ||
                        (m.HomeTeam.Code.Equals(opponentTeam.FifaCode, StringComparison.OrdinalIgnoreCase) && m.AwayTeam.Code.Equals(favTeam.FifaCode, StringComparison.OrdinalIgnoreCase))
                    );

                    // 3. Extract goals and print them directly to the txtResult TextBlock element
                    if (headToHeadMatch != null)
                    {
                        // We keep track of which team is home vs away to format the display score correctly
                        bool isFavHome = headToHeadMatch.HomeTeam.Code.Equals(favTeam.FifaCode, StringComparison.OrdinalIgnoreCase);

                        long favGoals = isFavHome ? headToHeadMatch.HomeTeam.Goals : headToHeadMatch.AwayTeam.Goals;
                        long oppGoals = isFavHome ? headToHeadMatch.AwayTeam.Goals : headToHeadMatch.HomeTeam.Goals;

                        txtResult.Text = $"{favGoals} : {oppGoals}";

                        // 4. NEXT STEP PLACEHOLDER: Trigger your player drawing logic on the pitch canvas here
                        // DrawLineupOnPitch(headToHeadMatch, favTeam.FifaCode);
                    }
                    else
                    {
                        txtResult.Text = "- : -";
                        pitchCanvas.Children.Clear();
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
                // 1. Turn on the background blur/dim filter
                DimOverlay.Visibility = Visibility.Visible;

                TeamInfoWindow infoWin = new TeamInfoWindow(selectedTeam);
                infoWin.Owner = this;
                infoWin.ShowDialog();

                // 2. Turn off the dimming filter once the popup is closed
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
                // 1. Turn on the background blur/dim filter
                DimOverlay.Visibility = Visibility.Visible;

                TeamInfoWindow infoWin = new TeamInfoWindow(selectedTeam);
                infoWin.Owner = this;
                infoWin.ShowDialog();

                // 2. Turn off the dimming filter once the popup is closed
                DimOverlay.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Please select an opponent national team first.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}