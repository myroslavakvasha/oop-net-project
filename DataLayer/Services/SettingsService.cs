using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Services
{
    public class SettingsService
    {
        private readonly string _path = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "..", "..", "..", "..", "DataLayer", "Resources", "Appsettings", "settings.txt")
        );

        public bool Exists => File.Exists(_path) && new FileInfo(_path).Length > 0;
        public string Title { get; private set; } = "World Cup 2018";
        public bool IsFullScreen { get; private set; } = false;
        public int Width { get; private set; } = 1280;
        public int Height { get; private set; } = 720;

        public SettingsService()
        {
            if (!File.Exists(_path)) return;
            Load();
        }

        public void Load()
        {
            if (!File.Exists(_path) || new FileInfo(_path).Length == 0) return;

            try
            {
                var lines = File.ReadAllLines(_path)
                    .Where(line => line.Contains("="))
                    .ToDictionary(
                        x => x.Split('=')[0].Trim().ToUpper(),
                        x => x.Split('=')[1].Trim()
                    );

                Title = lines.GetValueOrDefault("TITLE", "World Cup Statistics");
                IsFullScreen = lines.GetValueOrDefault("FULLSCREEN", "false")
                    .Equals("true", StringComparison.OrdinalIgnoreCase);

                if (lines.ContainsKey("WIDTH") && int.TryParse(lines["WIDTH"], out int w)) Width = w;
                if (lines.ContainsKey("HEIGHT") && int.TryParse(lines["HEIGHT"], out int h)) Height = h;
            }
            catch
            {
                // Fallback defaults if the file gets corrupted
                Title = "World Cup Statistics";
            }
        }

        public void Save(string title, bool isFullScreen, int width, int height)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);

            Title = title;
            IsFullScreen = isFullScreen;
            Width = width;
            Height = height;

            File.WriteAllLines(_path, new[]
            {
                $"TITLE={Title}",
                $"FULLSCREEN={IsFullScreen.ToString().ToLower()}",
                $"WIDTH={Width}",
                $"HEIGHT={Height}"
            });
        }
    }
}
