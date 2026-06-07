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
        private readonly string _currentUiTeam; // Stores the active UI selection

        // Added 'currentUiTeam' parameter here
        // ADD THIS FALLBACK CONSTRUCTOR:
        public SettingsWindow(SelectionService selectionService, SettingsService settingsService)
        {
            InitializeComponent();
            _selectionService = selectionService;
            _settingsService = settingsService;

            txtTitle.Text = _settingsService.Title;

            // Select correct championship gender item
            foreach (ComboBoxItem item in cbGender.Items)
            {
                if (item.Tag.ToString().Equals(_selectionService.Gender, StringComparison.OrdinalIgnoreCase))
                {
                    item.IsSelected = true;
                    break;
                }
            }

            // Select resolution item matching current size dimensions
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

                if (!foundMatch) cbResolution.SelectedIndex = 2; // Default 1024x720 fallback
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
                // 1. Title handling
                string newTitle = txtTitle.Text.Trim();
                if (string.IsNullOrEmpty(newTitle)) newTitle = "World Cup Statistics";

                // 2. Parse Gender selection
                var selectedGenderItem = (ComboBoxItem)cbGender.SelectedItem;
                string newGender = selectedGenderItem.Tag.ToString().ToLower();

                // 3. Keep the matching pre-saved gender specific team configuration
                string teamToPreserve = (newGender == "female") ? _selectionService.FemaleTeam : _selectionService.MaleTeam;

                // 4. Handle resolution tag metrics
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

                // 5. Commit states to file layers cleanly
                _selectionService.Save(newGender, teamToPreserve, _selectionService.FavoritesByTeam);
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