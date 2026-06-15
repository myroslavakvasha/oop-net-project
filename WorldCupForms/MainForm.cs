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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WorldCupForms
{
    public partial class MainForm : Form
    {
        private readonly ConfigService _configService;
        private readonly DataService _dataService;
        private readonly SelectionService _selectionService;
        private readonly SettingsService _settingsService;
        private bool _isReloading = false; // used when changing stuff in settings form

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
            _settingsService = new SettingsService();
            _dataService = new DataService(_configService, _selectionService);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            SetLoading(true);

            try
            {
                _settingsService.Load();
                this.Text = _settingsService.Title;

                var teams = await _dataService.GetTeamsAsync();
                var orderedTeams = teams.OrderBy(x => x.Country).ToList();

                cbTeams.SelectedIndexChanged -= cbTeams_SelectedIndexChanged;

                cbTeams.DataSource = orderedTeams;

                if (!string.IsNullOrEmpty(_selectionService.Team))
                {
                    var saved = orderedTeams.FirstOrDefault(t => t.FifaCode == _selectionService.Team);
                    if (saved != null)
                        cbTeams.SelectedItem = saved;
                }

                cbTeams.SelectedIndexChanged += cbTeams_SelectedIndexChanged;

                if (cbTeams.SelectedItem is Team selected)
                {
                    if (string.IsNullOrEmpty(_selectionService.Team))
                        _selectionService.Save(_selectionService.Gender, selected.FifaCode, string.Empty, _selectionService.FavoritesByTeam);

                    await LoadTeamDataAsync(selected);
                }
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
                _selectionService.Save(_selectionService.Gender, selected.FifaCode, string.Empty, _selectionService.FavoritesByTeam);
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

                var firstMatch = matches.First();
                var stats = firstMatch.HomeTeam.Code == team.FifaCode
                    ? firstMatch.HomeTeamStatistics
                    : firstMatch.AwayTeamStatistics;

                var allPlayers = stats.StartingEleven
                    .Union(stats.Substitutes)
                    .ToList();

                var favoritesForTeam = _selectionService.GetFavorites(team.FifaCode);

                foreach (var player in allPlayers)
                {
                    bool isFav = favoritesForTeam.Contains(player.Name);

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
            // currently displayed team's fifa code, not the saved one
            if (cbTeams.SelectedItem is not Team currentTeam) return;
            var currentFifaCode = currentTeam.FifaCode;

            var currentFavs = _selectionService.GetFavorites(currentFifaCode);

            if (currentFavs.Contains(player.Name))
            {
                currentFavs.Remove(player.Name);
                _selectionService.SaveFavorites(currentFifaCode, currentFavs);

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
                _selectionService.SaveFavorites(currentFifaCode, currentFavs);

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

        // checks if the dragged data is a PlayerCardControl; if yes, sets effect to Move, otherwise None
        // this is what makes the drop target "accept" the drag
        private void Panel_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PlayerCardControl)))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        // handles dropping onto the favorites panel
        private void FavoritesPanel_DragDrop(object? sender, DragEventArgs e)
        {
            var card = e.Data.GetData(typeof(PlayerCardControl)) as PlayerCardControl;
            if (card == null) return;
            if (flpFavorites.Controls.Contains(card)) return;

            var selectedCards = flpAllPlayers.Controls
                .OfType<PlayerCardControl>()
                .Where(c => c.IsSelected || c == card)
                .ToList();

            if (cbTeams.SelectedItem is not Team currentTeam) return;
            var currentFavs = _selectionService.GetFavorites(currentTeam.FifaCode);

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

            _selectionService.SaveFavorites(currentTeam.FifaCode, currentFavs);
        }

        // moves selected cards from favorites back to all-players, removes them from the saved favorites list
        private void AllPlayersPanel_DragDrop(object? sender, DragEventArgs e)
        {
            var card = e.Data.GetData(typeof(PlayerCardControl)) as PlayerCardControl;
            if (card == null) return;
            if (flpAllPlayers.Controls.Contains(card)) return;

            var selectedCards = flpFavorites.Controls
                .OfType<PlayerCardControl>()
                .Where(c => c.IsSelected || c == card)
                .ToList();

            if (cbTeams.SelectedItem is not Team currentTeam) return;
            var currentFavs = _selectionService.GetFavorites(currentTeam.FifaCode);

            foreach (var c in selectedCards)
            {
                flpFavorites.Controls.Remove(c);
                c.SetFavorite(false);
                c.SetSelected(false);
                flpAllPlayers.Controls.Add(c);
                currentFavs.Remove(c.Player.Name);
            }

            _selectionService.SaveFavorites(currentTeam.FifaCode, currentFavs);
        }

        private async void btnSettings_Click(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm())
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    _isReloading = true;

                    _settingsService.Load();
                    _selectionService.Load();

                    this.Text = _settingsService.Title;

                    flpFavorites.Controls.Clear();
                    flpAllPlayers.Controls.Clear();

                    cbTeams.SelectedIndexChanged -= cbTeams_SelectedIndexChanged;
                    cbTeams.DataSource = null;

                    SetLoading(true);
                    try
                    {
                        var teams = await _dataService.GetTeamsAsync();
                        var orderedTeams = teams.OrderBy(x => x.Country).ToList();

                        cbTeams.DataSource = orderedTeams;

                        string savedTeamCode = _selectionService.Team;

                        if (!string.IsNullOrEmpty(savedTeamCode))
                        {
                            var saved = orderedTeams.FirstOrDefault(t => t.FifaCode == savedTeamCode);
                            if (saved != null)
                                cbTeams.SelectedItem = saved;
                        }
                        else
                        {
                            cbTeams.SelectedIndex = 0;
                        }

                        cbTeams.SelectedIndexChanged += cbTeams_SelectedIndexChanged;
                        _isReloading = false;

                        if (cbTeams.SelectedItem is Team selected)
                            await LoadTeamDataAsync(selected);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reloading championships: {ex.Message}");
                        _isReloading = false;
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
            if (e.CloseReason == CloseReason.TaskManagerClosing || e.CloseReason == CloseReason.WindowsShutDown)
            {
                return;
            }

            DialogResult result = MessageBox.Show(
                this,
                "Are you sure you want to close the application?",
                "Exit Application Confirmation",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1
            );

            if (result == DialogResult.No || result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }
}