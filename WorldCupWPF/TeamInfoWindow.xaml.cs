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

            txtCountryName.Text = team.Country;
            txtFifaCode.Text = $"FIFA Code: {team.FifaCode}";

            txtGames.Text = team.GamesPlayed.ToString();
            txtWins.Text = team.Wins.ToString();
            txtLosses.Text = team.Losses.ToString();
            txtDraws.Text = team.Draws.ToString();

            txtGoalsScored.Text = team.GoalsFor.ToString();
            txtGoalsConceded.Text = team.GoalsAgainst.ToString();
            txtGoalDiff.Text = team.GoalDifferential.ToString();

            var imgTeamControl = this.FindName("imgTeam") as System.Windows.Controls.Image;
            if (imgTeamControl != null)
            {
                try
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                    string solutionRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDir, @"..\..\..\..\"));

                    string imagesFolder = System.IO.Path.Combine(solutionRoot, "DataLayer", "Resources", "img", "players");

                    string targetFileName = $"{team.FifaCode.ToUpper()}.png";
                    string imagePath = System.IO.Path.Combine(imagesFolder, targetFileName);

                    if (System.IO.File.Exists(imagePath))
                    {
                        imgTeamControl.Source = new BitmapImage(new Uri(imagePath));
                    }
                    else
                    {
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