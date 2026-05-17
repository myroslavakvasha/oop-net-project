using DataLayer.Services;

namespace WorldCupForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var selectionService = new SelectionService();
            if (!selectionService.Exists)
            {
                // Show the initial settings form as a dialog
                using (var startupForm = new StartupForm(selectionService))
                {
                    if (startupForm.ShowDialog() != DialogResult.OK)
                    {
                        // If user closes the setup without saving, exit the app
                        return;
                    }
                }
            }

            Application.Run(new MainForm());

        }
    }
}