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
        private string _gender; // FIX: Added private field to hold the gender context safely

        public PlayerControl()
        {
            InitializeComponent();

            // Wire up the Loaded event so we only update the UI elements 
            // AFTER WPF has finished creating them from XAML!
            this.Loaded += PlayerControl_Loaded;
        }

        public void PopulatePlayerData(Player player, string gender, string countryCode)
        {
            // 1. Save data fields immediately so the click interaction and image renderer have the correct info
            _playerData = player;
            _countryCode = countryCode;
            _gender = gender; // FIX: Assigning the passed string parameter to our private field

            if (player == null) return;

            // 2. Set the DataContext immediately for binding fallback support
            this.DataContext = player;

            // 3. If the control is already loaded visually, run the update immediately
            if (this.IsLoaded)
            {
                ApplyDataToUi();
            }
        }

        private void PlayerControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Run the UI updates safely now that XAML blocks are built
            ApplyDataToUi();
        }

        private void ApplyDataToUi()
        {
            if (_playerData == null) return;

            // 1. Map Text Properties safely
            if (txtPlayerName != null)
            {
                txtPlayerName.Text = _playerData.Name;
            }

            if (txtShirtNumber != null)
            {
                txtShirtNumber.Text = _playerData.ShirtNumber.ToString();
            }

            // 2. Load Local Custom Player Image from DataLayer folder structure
            if (imgPlayer != null)
            {
                try
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                    // Navigate out of bin/Debug/netX.X/WorldCupWPF to get to the solution root
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

                // CHANGE THIS: Pass _gender right into the constructor at the end
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