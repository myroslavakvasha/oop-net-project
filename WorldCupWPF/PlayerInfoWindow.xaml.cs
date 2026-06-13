using DataLayer.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WorldCupWPF
{
    public partial class PlayerInfoWindow : Window
    {
        private Player _playerData;
        private string _countryCode;
        private string _gender;
        private object _activeMatch;

        public PlayerInfoWindow(Player player, object activeMatch, string countryCode, string gender)
        {
            InitializeComponent();

            _playerData = player;
            _activeMatch = activeMatch;
            _countryCode = countryCode;
            _gender = gender;

            PopulatePlayerDetails();
        }

        private void PopulatePlayerDetails()
        {
            if (_playerData == null) return;

            if (txtPlayerName != null) txtPlayerName.Text = _playerData.Name;
            if (txtPosition != null) txtPosition.Text = _playerData.Position.ToString();
            if (txtShirtNumber != null) txtShirtNumber.Text = _playerData.ShirtNumber.ToString();

            if (brdCaptainBadge != null)
            {
                brdCaptainBadge.Visibility = _playerData.Captain ? Visibility.Visible : Visibility.Collapsed;
            }

            int goalsCount = 0;
            int yellowCardsCount = 0;

            if (_activeMatch is Match match)
            {
                var allEvents = (match.HomeTeamEvents ?? new List<TeamEvent>())
                    .Concat(match.AwayTeamEvents ?? new List<TeamEvent>());

                goalsCount = allEvents.Count(ev =>
                    (ev.TypeOfEvent == DataLayer.Models.Enums.TypeOfEvent.Goal ||
                     ev.TypeOfEvent == DataLayer.Models.Enums.TypeOfEvent.GoalPenalty) &&
                    ev.Player != null &&
                    ev.Player.Equals(_playerData.Name, StringComparison.OrdinalIgnoreCase));

                yellowCardsCount = allEvents.Count(ev =>
                    ev.TypeOfEvent == DataLayer.Models.Enums.TypeOfEvent.YellowCard &&
                    ev.Player != null &&
                    ev.Player.Equals(_playerData.Name, StringComparison.OrdinalIgnoreCase));
            }

            if (txtMatchGoals != null) txtMatchGoals.Text = goalsCount.ToString();
            if (txtYellowCards != null) txtYellowCards.Text = yellowCardsCount.ToString();

            if (imgPlayerPhoto != null)
            {
                try
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    string solutionRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDir, @"..\..\..\..\"));
                    string imagesFolder = System.IO.Path.Combine(solutionRoot, "DataLayer", "Resources", "img", "players");

                    string cleanGender = string.IsNullOrEmpty(_gender) ? "male" : _gender.Trim().ToLower();
                    string targetFileName = $"{cleanGender}_{_countryCode}_{_playerData.ShirtNumber}.png";
                    string imagePath = System.IO.Path.Combine(imagesFolder, targetFileName);

                    if (System.IO.File.Exists(imagePath))
                    {
                        imgPlayerPhoto.Source = new BitmapImage(new Uri(imagePath));
                    }
                    else
                    {
                        string defaultPath = System.IO.Path.Combine(imagesFolder, "default.png");
                        if (System.IO.File.Exists(defaultPath))
                        {
                            imgPlayerPhoto.Source = new BitmapImage(new Uri(defaultPath));
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Modal image load failed: {ex.Message}");
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}