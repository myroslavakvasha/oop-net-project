using DataLayer.Models;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WorldCupWPF
{
    /// <summary>
    /// Interaction logic for TeamInfoWindow.xaml
    /// </summary>
    public partial class TeamInfoWindow : Window
    {
        public TeamInfoWindow(Team team)
        {
            InitializeComponent();
            PopulateTeamData(team);
        }

        private void PopulateTeamData(Team team)
        {
            if (team == null) return;

            // 1. Core Team Details
            txtCountryName.Text = team.Country;
            txtFifaCode.Text = $"FIFA Code: {team.FifaCode}";

            txtGames.Text = team.GamesPlayed.ToString();
            txtWins.Text = team.Wins.ToString();
            txtLosses.Text = team.Losses.ToString();
            txtDraws.Text = team.Draws.ToString();

            txtGoalsScored.Text = team.GoalsFor.ToString();
            txtGoalsConceded.Text = team.GoalsAgainst.ToString();
            txtGoalDiff.Text = team.GoalDifferential.ToString();

            // 2. Load Dynamic Team Image / Flag Safely
            // Note: Make sure your TeamInfoWindow.xaml has an Image control named "imgTeam"
            var imgTeamControl = this.FindName("imgTeam") as System.Windows.Controls.Image;
            if (imgTeamControl != null)
            {
                try
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                    // Navigate past build outputs to the solution root
                    string solutionRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDir, @"..\..\..\..\"));

                    // Target your shared players/images asset route
                    string imagesFolder = System.IO.Path.Combine(solutionRoot, "DataLayer", "Resources", "img", "players");

                    // Pattern: Looks for a file named after the FIFA code (e.g., "CRO.png")
                    // If your flags are in a different folder, just alter the path above!
                    string targetFileName = $"{team.FifaCode.ToUpper()}.png";
                    string imagePath = System.IO.Path.Combine(imagesFolder, targetFileName);

                    if (System.IO.File.Exists(imagePath))
                    {
                        imgTeamControl.Source = new BitmapImage(new Uri(imagePath));
                    }
                    else
                    {
                        // Fallback to default asset if explicit image doesn't exist
                        string defaultPath = System.IO.Path.Combine(imagesFolder, "default.png");
                        if (System.IO.File.Exists(defaultPath))
                        {
                            imgTeamControl.Source = new BitmapImage(new Uri(defaultPath));
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Team image load failed: {ex.Message}");
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}