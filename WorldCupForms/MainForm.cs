using DataLayer.Models;
using DataLayer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorldCupForms
{
    public partial class MainForm : Form
    {
        private readonly ConfigService _configService;
        private readonly DataService _dataService;
        private readonly SelectionService _selectionService;
        private bool _isReloading = false;
        public MainForm()
        {
            InitializeComponent();

            flpFavorites.AllowDrop = true;
            flpAllPlayers.AllowDrop = true;

            flpFavorites.DragEnter += Panel_DragEnter;
            flpAllPlayers.DragEnter += Panel_DragEnter;

            flpFavorites.DragDrop += FavoritesPanel_DragDrop;
            flpAllPlayers.DragDrop += AllPlayersPanel_DragDrop;

            _configService = new ConfigService();
            _selectionService = new SelectionService();
            _dataService = new DataService(_configService, _selectionService);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            SetLoading(true);

            try
            {
                var teams = await _dataService.GetTeamsAsync();
                var orderedTeams = teams.OrderBy(x => x.Country).ToList();

                cbTeams.SelectedIndexChanged -= cbTeams_SelectedIndexChanged;

                cbTeams.DataSource = orderedTeams;

                // pre-select saved team if exists
                if (!string.IsNullOrEmpty(_selectionService.Team))
                {
                    var saved = orderedTeams.FirstOrDefault(t => t.FifaCode == _selectionService.Team);
                    if (saved != null)
                        cbTeams.SelectedItem = saved;
                }

                cbTeams.SelectedIndexChanged += cbTeams_SelectedIndexChanged;

                if (cbTeams.SelectedItem is Team selected)
                    await LoadTeamDataAsync(selected);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teams: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void SetLoading(bool isLoading)
        {
            pbSpinner.Visible = isLoading;

            flpFavorites.Visible = !isLoading;
            flpAllPlayers.Visible = !isLoading;
        }

        private async void cbTeams_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isReloading) return;

            if (cbTeams.SelectedItem is Team selected)
            {
                _selectionService.Save(_selectionService.Gender, selected.FifaCode, _selectionService.FavoritesByTeam);
                await LoadTeamDataAsync(selected);
            }
        }

        private async Task LoadTeamDataAsync(Team team)
        {
            SetLoading(true);

            flpFavorites.Controls.Clear();
            flpAllPlayers.Controls.Clear();

            try
            {
                var matches = await _dataService.GetMatchesByTeamAsync(team.FifaCode);
                if (!matches.Any())
                {
                    MessageBox.Show("No matches found for this team.");
                    return;
                }

                // get first match and find the correct statistics for our team
                var firstMatch = matches.First();
                var stats = firstMatch.HomeTeam.Code == team.FifaCode
                    ? firstMatch.HomeTeamStatistics
                    : firstMatch.AwayTeamStatistics;

                // union of starting eleven and substitutes
                var allPlayers = stats.StartingEleven
                    .Union(stats.Substitutes)
                    .ToList();

                var favoritesForTeam = _selectionService.GetFavorites(team.FifaCode);

                foreach (var player in allPlayers)
                {
                    bool isFav = favoritesForTeam.Contains(player.Name);

                    // Pass the active gender from selection service here:
                    var card = new PlayerCardControl(player, isFav, team.FifaCode, _selectionService.Gender);
                    card.FavoriteToggled += OnFavoriteToggled;

                    if (isFav)
                        flpFavorites.Controls.Add(card);
                    else
                        flpAllPlayers.Controls.Add(card);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading team data: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }
        private void OnFavoriteToggled(object? sender, Player player)
        {
            var currentTeam = _selectionService.Team;
            var currentFavs = _selectionService.GetFavorites(currentTeam);

            if (currentFavs.Contains(player.Name))
            {
                currentFavs.Remove(player.Name);
                _selectionService.SaveFavorites(currentTeam, currentFavs);

                var card = flpFavorites.Controls
                    .OfType<PlayerCardControl>()
                    .FirstOrDefault(c => c.Player.Equals(player));

                if (card != null)
                {
                    flpFavorites.Controls.Remove(card);
                    card.SetFavorite(false);
                    flpAllPlayers.Controls.Add(card);
                }
            }
            else
            {
                if (currentFavs.Count >= 3)
                {
                    MessageBox.Show("You can only have 3 favorite players.");
                    return;
                }

                currentFavs.Add(player.Name);
                _selectionService.SaveFavorites(currentTeam, currentFavs);

                var card = flpAllPlayers.Controls
                    .OfType<PlayerCardControl>()
                    .FirstOrDefault(c => c.Player.Equals(player));

                if (card != null)
                {
                    flpAllPlayers.Controls.Remove(card);
                    card.SetFavorite(true);
                    flpFavorites.Controls.Add(card);
                }
            }
        }
        private void Panel_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PlayerCardControl)))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void FavoritesPanel_DragDrop(object? sender, DragEventArgs e)
        {
            var card = e.Data.GetData(typeof(PlayerCardControl)) as PlayerCardControl;
            if (card == null) return;
            if (flpFavorites.Controls.Contains(card)) return;

            // get all selected cards from all players panel
            var selectedCards = flpAllPlayers.Controls
                .OfType<PlayerCardControl>()
                .Where(c => c.IsSelected || c == card)
                .ToList();

            var currentTeam = (_selectionService.Team);
            var currentFavs = _selectionService.GetFavorites(currentTeam);

            if (currentFavs.Count + selectedCards.Count > 3)
            {
                MessageBox.Show("You can only have 3 favorite players.");
                return;
            }

            foreach (var c in selectedCards)
            {
                flpAllPlayers.Controls.Remove(c);
                c.SetFavorite(true);
                c.SetSelected(false);
                flpFavorites.Controls.Add(c);
                currentFavs.Add(c.Player.Name);
            }

            _selectionService.SaveFavorites(currentTeam, currentFavs);
        }

        private void AllPlayersPanel_DragDrop(object? sender, DragEventArgs e)
        {
            var card = e.Data.GetData(typeof(PlayerCardControl)) as PlayerCardControl;
            if (card == null) return;
            if (flpAllPlayers.Controls.Contains(card)) return;

            // get all selected cards from favorites panel
            var selectedCards = flpFavorites.Controls
                .OfType<PlayerCardControl>()
                .Where(c => c.IsSelected || c == card)
                .ToList();

            var currentTeam = _selectionService.Team;
            var currentFavs = _selectionService.GetFavorites(currentTeam);

            foreach (var c in selectedCards)
            {
                flpFavorites.Controls.Remove(c);
                c.SetFavorite(false);
                c.SetSelected(false);
                flpAllPlayers.Controls.Add(c);
                currentFavs.Remove(c.Player.Name);
            }

            _selectionService.SaveFavorites(currentTeam, currentFavs);
        }

        private async void btnSettings_Click(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm())
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    // Activate our safety guard flag
                    _isReloading = true;

                    // 1. Force SelectionService to reload from the text file
                    _selectionService.Load();

                    // 2. Completely clean out the UI layout components to avoid remnants
                    flpFavorites.Controls.Clear();
                    flpAllPlayers.Controls.Clear();

                    // Unhook event explicitly
                    cbTeams.SelectedIndexChanged -= cbTeams_SelectedIndexChanged;
                    cbTeams.DataSource = null;

                    SetLoading(true);
                    try
                    {
                        var teams = await _dataService.GetTeamsAsync();
                        var orderedTeams = teams.OrderBy(x => x.Country).ToList();

                        cbTeams.DataSource = orderedTeams;

                        // Pre-select the saved team for the active gender we just switched to
                        string savedTeamCode = _selectionService.Team;

                        if (!string.IsNullOrEmpty(savedTeamCode))
                        {
                            var saved = orderedTeams.FirstOrDefault(t => t.FifaCode == savedTeamCode);
                            if (saved != null)
                            {
                                cbTeams.SelectedItem = saved;
                            }
                        }
                        else
                        {
                            // If no team was saved for this gender yet, default to the first one safely
                            cbTeams.SelectedIndex = 0;
                        }

                        // Re-hook the event handler safely now that selections are aligned
                        cbTeams.SelectedIndexChanged += cbTeams_SelectedIndexChanged;

                        // Turn off the safety guard flag right before fetching the team profiles
                        _isReloading = false;

                        if (cbTeams.SelectedItem is Team selected)
                        {
                            await LoadTeamDataAsync(selected);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reloading championships: {ex.Message}");
                        _isReloading = false; // Safety fallback
                    }
                    finally
                    {
                        SetLoading(false);
                    }
                }
            }
        }

        private async void btnViewRankings_Click(object sender, EventArgs e)
        {
            if (cbTeams.SelectedItem is Team selectedTeam)
            {
                SetLoading(true);

                try
                {
                    var (players, matches) = await _dataService.GetTeamRankingsAsync(selectedTeam.FifaCode);

                    using (RankingsForm rankingsForm = new RankingsForm(players, matches))
                    {
                        rankingsForm.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"An error occurred while loading team statistics: {ex.Message}",
                        "Data Load Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    SetLoading(false);
                }
            }
            else
            {
                MessageBox.Show("Please select a valid national team first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If the close reason is already controlled or shutting down from Task Manager, let it pass
            if (e.CloseReason == CloseReason.TaskManagerClosing || e.CloseReason == CloseReason.WindowsShutDown)
            {
                return;
            }

            // Display confirmation dialog box targeted directly at the active Form layer
            DialogResult result = MessageBox.Show(
                this,
                "Are you sure you want to close the application?",
                "Exit Application Confirmation",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1 // Defaults the focus context to "Yes" (Enter Key maps here natively)
            );

            if (result == DialogResult.No || result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }

}
