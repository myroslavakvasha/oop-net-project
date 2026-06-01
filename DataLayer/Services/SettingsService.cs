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
            Dictionary<string, string> lines = File.ReadAllLines(_path).ToDictionary(
                x => x.Split("=")[0].Trim(),
                x => x.Split("=")[1].Trim()
                );

            Title = lines.GetValueOrDefault("TITLE", Title);
            IsFullScreen = lines.GetValueOrDefault("FULLSCREEN", "false")
                .Equals("true", StringComparison.OrdinalIgnoreCase);

            if (int.TryParse(lines.GetValueOrDefault("WIDTH"), out int w)) Width = w;
            if (int.TryParse(lines.GetValueOrDefault("HEIGHT"), out int h)) Height = h;
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
