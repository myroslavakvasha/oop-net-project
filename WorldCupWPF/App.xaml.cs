using DataLayer.Services;
using System.Configuration;
using System.Data;
using System.Windows;

namespace WorldCupWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var configService = new ConfigService();
            var selectionService = new SelectionService();
            var settingsService = new SettingsService();

            // 1. If selection data doesn't exist, open settings instantly
            if (!selectionService.Exists || string.IsNullOrEmpty(selectionService.Gender))
            {
                SettingsWindow settingsDialog = new SettingsWindow(selectionService, settingsService);

                bool? dialogResult = settingsDialog.ShowDialog();

                // If they close or cancel, stop execution entirely
                if (dialogResult != true)
                {
                    Application.Current.Shutdown();
                    return;
                }

                // Force refresh local data structures out of the text files immediately
                selectionService.Load();
                settingsService.Load(); // Ensure display metrics are updated too
            }

            // 2. AUTOMATIC TRANSITION: This runs immediately after settingsDialog returns true
            MainWindow mainWindow = new MainWindow(configService, selectionService, settingsService);

            // Apply resolution configurations saved out of preferences 
            if (settingsService.IsFullScreen)
            {
                mainWindow.WindowState = WindowState.Maximized;
                mainWindow.WindowStyle = WindowStyle.None;
            }
            else
            {
                // Fallback to saved preferences or sensible defaults
                mainWindow.Width = settingsService.Width > 0 ? settingsService.Width : 1280;
                mainWindow.Height = settingsService.Height > 0 ? settingsService.Height : 720;
            }

            // Reveal the main dashboard window to the user
            mainWindow.Show();
        }
    }

}
