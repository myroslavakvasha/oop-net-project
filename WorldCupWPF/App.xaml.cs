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

            // Prevent WPF from shutting down when SettingsWindow closes
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var configService = new ConfigService();
            var selectionService = new SelectionService();
            var settingsService = new SettingsService();

            if (!selectionService.Exists || string.IsNullOrEmpty(selectionService.Gender) || !settingsService.Exists)
            {
                var settingsDialog = new SettingsWindow(selectionService, settingsService);
                if (settingsDialog.ShowDialog() != true)
                {
                    Shutdown();
                    return;
                }
                selectionService.Load();
                settingsService.Load();
            }

            var mainWindow = new MainWindow(configService, selectionService, settingsService);
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (settingsService.IsFullScreen)
                mainWindow.WindowState = WindowState.Maximized;
            else
            {
                mainWindow.Width = settingsService.Width > 0 ? settingsService.Width : 1280;
                mainWindow.Height = settingsService.Height > 0 ? settingsService.Height : 720;
            }

            // Now hand control back to normal shutdown behavior
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
        }
    }

}
