using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Services
{
    public class SelectionService
    {
        private readonly string _path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources", "Appsettings", "selections.txt");

        public bool Exists => File.Exists(_path);
        public string Gender { get; private set; } = string.Empty;
        public string Team { get; private set; } = string.Empty;
        public List<string> Players { get; private set; } = new();
        public SelectionService()
        {
            if (!Exists) return;
            Load();
        }

        private void Load()
        {
            Dictionary<string, string> lines = File.ReadAllLines(_path).ToDictionary(
                x => x.Split("=")[0].Trim(),
                x => x.Split("=")[1].Trim()
                );

            Gender = lines.GetValueOrDefault("GENDER", string.Empty);
            Team = lines.GetValueOrDefault("TEAM", string.Empty);

            Players = new();
            for (int i = 1; i <= 3; i++)
            {
                if (lines.TryGetValue($"PLAYER{i}", out var player) && !string.IsNullOrEmpty(player))
                    Players.Add(player);
            }
        }

        public void Save(string gender, string team, List<string> players)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);

            Gender = gender;
            Team = team;
            Players = players.Take(3).ToList();

            var lines = new List<string>
            {
                $"GENDER={Gender}",
                $"TEAM={Team}"
            };

            for (int i = 0; i < Players.Count; i++)
                lines.Add($"PLAYER{i + 1}={Players[i]}");

            File.WriteAllLines(_path, lines);
        }
    }
}
