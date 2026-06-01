//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DataLayer.Services
//{
//    public class SelectionService
//    {
//        private readonly string _path = Path.Combine(
//            AppDomain.CurrentDomain.BaseDirectory,
//            "Resources", "Appsettings", "selections.txt");

//        public bool Exists => File.Exists(_path);
//        public string Gender { get; private set; } = string.Empty;
//        public string Team { get; private set; } = string.Empty;
//        public List<string> Players { get; private set; } = new();

//        public Dictionary<string, List<string>> FavoritesByTeam { get; private set; } = new();

//        public List<string> GetFavorites(string fifaCode)
//            => FavoritesByTeam.GetValueOrDefault(fifaCode, new List<string>());
//        public SelectionService()
//        {
//            if (!Exists) return;
//            Load();
//        }
//        private void Load()
//        {
//            var lines = File.ReadAllLines(_path).ToDictionary(
//                x => x.Split("=")[0].Trim(),
//                x => x.Split("=")[1].Trim()
//            );

//            Gender = lines.GetValueOrDefault("GENDER", string.Empty);
//            Team = lines.GetValueOrDefault("TEAM", string.Empty);

//            FavoritesByTeam = new();
//            foreach (var kvp in lines.Where(l => l.Key.StartsWith("PLAYERS_")))
//            {
//                var code = kvp.Key.Replace("PLAYERS_", "");
//                var players = kvp.Value.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
//                FavoritesByTeam[code] = players;
//            }
//        }

//        public void SaveFavorites(string fifaCode, List<string> players)
//        {
//            FavoritesByTeam[fifaCode] = players.Take(3).ToList();
//            SaveAll();
//        }

//        public void Save(string gender, string team, Dictionary<string, List<string>> favoritesByTeam)
//        {
//            Gender = gender;
//            Team = team;
//            FavoritesByTeam = favoritesByTeam;
//            SaveAll();
//        }

//        private void SaveAll()
//        {
//            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);

//            var lines = new List<string>
//                {
//                    $"GENDER={Gender}",
//                    $"TEAM={Team}"
//                };

//            foreach (var kvp in FavoritesByTeam)
//                lines.Add($"PLAYERS_{kvp.Key}={string.Join(",", kvp.Value)}");

//            File.WriteAllLines(_path, lines);
//        }
//    }
//}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataLayer.Services
{
    public class SelectionService
    {
        private readonly string _path = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "..", "..", "..", "..", "DataLayer", "Resources", "Appsettings", "selections.txt")
        );

        public bool Exists => File.Exists(_path);
        public string Gender { get; private set; } = string.Empty;
        public string MaleTeam { get; private set; } = string.Empty;
        public string FemaleTeam { get; private set; } = string.Empty;

        // Dynamic property returning the team matching the active gender
        public string Team => Gender == "female" ? FemaleTeam : MaleTeam;

        // Keys inside here will now store composite values like "male_ENG" or "female_ENG"
        public Dictionary<string, List<string>> FavoritesByTeam { get; private set; } = new();

        public List<string> GetFavorites(string fifaCode)
            => FavoritesByTeam.GetValueOrDefault($"{Gender}_{fifaCode}", new List<string>());

        public SelectionService()
        {
            if (!Exists) return;
            Load();
        }

        public void Load()
        {
            var lines = File.ReadAllLines(_path)
                .Select(x => x.Split('='))
                .Where(x => x.Length == 2)
                .ToDictionary(x => x[0].Trim(), x => x[1].Trim());

            Gender = lines.GetValueOrDefault("GENDER", string.Empty);
            MaleTeam = lines.GetValueOrDefault("MALE_TEAM", string.Empty);
            FemaleTeam = lines.GetValueOrDefault("FEMALE_TEAM", string.Empty);

            FavoritesByTeam = new();
            foreach (var kvp in lines.Where(l => l.Key.StartsWith("PLAYERS_")))
            {
                var uniqueKey = kvp.Key.Replace("PLAYERS_", ""); // e.g., "male_ENG"
                var players = kvp.Value.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
                FavoritesByTeam[uniqueKey] = players;
            }
        }

        public void SaveFavorites(string fifaCode, List<string> players)
        {
            FavoritesByTeam[$"{Gender}_{fifaCode}"] = players.Take(3).ToList();
            SaveAll();
        }

        public void Save(string gender, string team, Dictionary<string, List<string>> favoritesByTeam)
        {
            Gender = gender;
            if (gender == "female")
                FemaleTeam = team;
            else
                MaleTeam = team;

            FavoritesByTeam = favoritesByTeam;
            SaveAll();
        }

        private void SaveAll()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);

            var lines = new List<string>
            {
                $"GENDER={Gender}",
                $"MALE_TEAM={MaleTeam}",
                $"FEMALE_TEAM={FemaleTeam}"
            };

            foreach (var kvp in FavoritesByTeam)
                lines.Add($"PLAYERS_{kvp.Key}={string.Join(",", kvp.Value)}");

            File.WriteAllLines(_path, lines);
        }
    }
}