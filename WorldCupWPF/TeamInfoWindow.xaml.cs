using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
