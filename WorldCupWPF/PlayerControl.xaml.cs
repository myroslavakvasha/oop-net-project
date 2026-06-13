using DataLayer.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WorldCupWPF
{
    public partial class PlayerControl : UserControl
    {
        private Player _playerData;
        private string _countryCode;
        private string _gender;

        public PlayerControl()
        {
            InitializeComponent();

            this.Loaded += PlayerControl_Loaded;
        }

        public void PopulatePlayerData(Player player, string gender, string countryCode)
        {
            _playerData = player;
            _countryCode = countryCode;
            _gender = gender;

            if (player == null) return;

            this.DataContext = player;

            if (this.IsLoaded)
            {
                ApplyDataToUi();
            }
        }

        private void PlayerControl_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyDataToUi();
        }

        private void ApplyDataToUi()
        {
            if (_playerData == null) return;

            if (txtPlayerName != null)
            {
                txtPlayerName.Text = _playerData.Name;
            }

            if (txtShirtNumber != null)
            {
                txtShirtNumber.Text = _playerData.ShirtNumber.ToString();
            }

            if (imgPlayer != null)
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
                        imgPlayer.Source = new BitmapImage(new Uri(imagePath));
                    }
                    else
                    {

                        string defaultPath = System.IO.Path.Combine(imagesFolder, "default.png");
                        if (System.IO.File.Exists(defaultPath))
                        {
                            imgPlayer.Source = new BitmapImage(new Uri(defaultPath));
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Image load failed: {ex.Message}");
                }
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            MainWindow mainWin = Window.GetWindow(this) as MainWindow;

            if (mainWin != null && _playerData != null)
            {
                if (mainWin.MainWindowDimOverlay != null)
                {
                    mainWin.MainWindowDimOverlay.Visibility = Visibility.Visible;
                }

                PlayerInfoWindow playerInfoWin = new PlayerInfoWindow(_playerData, mainWin.CurrentActiveMatch, _countryCode, _gender)
                {
                    Owner = mainWin
                };

                playerInfoWin.ShowDialog();

                if (mainWin.MainWindowDimOverlay != null)
                {
                    mainWin.MainWindowDimOverlay.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}