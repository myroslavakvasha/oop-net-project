using System;
using System.Windows;
using System.Windows.Controls;
using DataLayer.Services;

namespace WorldCupWPF
{
    public partial class SettingsWindow : Window
    {
        private readonly SelectionService _selectionService;
        private readonly SettingsService _settingsService;

        public SettingsWindow(SelectionService selectionService, SettingsService settingsService)
        {
            InitializeComponent();
            _selectionService = selectionService;
            _settingsService = settingsService;

            txtTitle.Text = _settingsService.Title;

            foreach (ComboBoxItem item in cbGender.Items)
            {
                if (item.Tag.ToString().Equals(_selectionService.Gender, StringComparison.OrdinalIgnoreCase))
                {
                    item.IsSelected = true;
                    break;
                }
            }

            if (_settingsService.IsFullScreen)
            {
                cbResolution.SelectedIndex = 0;
            }
            else
            {
                string currentResTag = $"{_settingsService.Width}x{_settingsService.Height}";
                bool foundMatch = false;

                foreach (ComboBoxItem item in cbResolution.Items)
                {
                    if (item.Tag.ToString().Equals(currentResTag, StringComparison.OrdinalIgnoreCase))
                    {
                        item.IsSelected = true;
                        foundMatch = true;
                        break;
                    }
                }

                if (!foundMatch) cbResolution.SelectedIndex = 2;
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Do you want to save these changes?",
                "Confirm Changes",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                string newTitle = txtTitle.Text.Trim();
                if (string.IsNullOrEmpty(newTitle)) newTitle = "World Cup Statistics";

                var selectedGenderItem = (ComboBoxItem)cbGender.SelectedItem;
                string newGender = selectedGenderItem.Tag.ToString().ToLower();

                string teamToPreserve = (newGender == "female") ? _selectionService.FemaleTeam : _selectionService.MaleTeam;

                string opponentToPreserve = (newGender == "female") ? _selectionService.FemaleOpponent : _selectionService.MaleOpponent;

                var selectedResItem = (ComboBoxItem)cbResolution.SelectedItem;
                string resTag = selectedResItem.Tag.ToString();

                bool isFullScreen = resTag == "fullscreen";
                int width = _settingsService.Width;
                int height = _settingsService.Height;

                if (!isFullScreen && resTag.Contains("x"))
                {
                    var parts = resTag.Split('x');
                    int.TryParse(parts[0], out width);
                    int.TryParse(parts[1], out height);
                }

                _selectionService.Save(newGender, teamToPreserve, opponentToPreserve, _selectionService.FavoritesByTeam);
                _settingsService.Save(newTitle, isFullScreen, width, height);

                this.DialogResult = true;
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}