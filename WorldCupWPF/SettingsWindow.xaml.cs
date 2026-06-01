using DataLayer.Services;
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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly SelectionService _selectionService;
        private readonly SettingsService _settingsService;

        public SettingsWindow(SelectionService selectionService, SettingsService settingsService)
        {
            InitializeComponent();
            _selectionService = selectionService;
            _settingsService = settingsService;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var confirmResult = MessageBox.Show("Do you want to save these settings?", "Confirm Settings", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmResult == MessageBoxResult.No) return;

            // 1. Gather gender selection
            string selectedGender = (cbGender.SelectedItem as ComboBoxItem)?.Tag.ToString() ?? "male";

            // 2. Gather resolution selection
            ComboBoxItem resItem = cbResolution.SelectedItem as ComboBoxItem;
            bool isFullScreen = resItem?.Tag.ToString() == "fullscreen";

            int width = 1280;
            int height = 720;

            if (!isFullScreen && resItem != null)
            {
                string[] parts = resItem.Tag.ToString().Split('x');
                int.TryParse(parts[0], out width);
                int.TryParse(parts[1], out height);
            }

            // 3. Save settings cleanly to your text configuration structures
            _settingsService.Save($"World Cup ({selectedGender})", isFullScreen, width, height);
            _selectionService.Save(selectedGender, _selectionService.Team, _selectionService.FavoritesByTeam);

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
