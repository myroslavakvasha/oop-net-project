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

        public string MaleOpponent { get; private set; } = string.Empty;
        public string FemaleOpponent { get; private set; } = string.Empty;

        public string Team => Gender == "female" ? FemaleTeam : MaleTeam;
        public string OpponentTeam => Gender == "female" ? FemaleOpponent : MaleOpponent;

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
            if (!Exists) return;

            var lines = File.ReadAllLines(_path)
                .Select(x => x.Split('='))
                .Where(x => x.Length == 2)
                .ToDictionary(x => x[0].Trim(), x => x[1].Trim());

            Gender = lines.GetValueOrDefault("GENDER", string.Empty);
            MaleTeam = lines.GetValueOrDefault("MALE_TEAM", string.Empty);
            FemaleTeam = lines.GetValueOrDefault("FEMALE_TEAM", string.Empty);
            MaleOpponent = lines.GetValueOrDefault("MALE_OPPONENT", string.Empty);
            FemaleOpponent = lines.GetValueOrDefault("FEMALE_OPPONENT", string.Empty);

            FavoritesByTeam = new();
            foreach (var kvp in lines.Where(l => l.Key.StartsWith("PLAYERS_")))
            {
                var uniqueKey = kvp.Key.Replace("PLAYERS_", "");
                var players = kvp.Value.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
                FavoritesByTeam[uniqueKey] = players;
            }
        }

        public void SaveFavorites(string fifaCode, List<string> players)
        {
            FavoritesByTeam[$"{Gender}_{fifaCode}"] = players.Take(3).ToList();
            SaveAll();
        }

        public void Save(string gender, string team, string opponentTeam, Dictionary<string, List<string>> favoritesByTeam)
        {
            Gender = gender;

            if (gender == "female")
            {
                FemaleTeam = team;
                FemaleOpponent = opponentTeam;
            }
            else
            {
                MaleTeam = team;
                MaleOpponent = opponentTeam;
            }

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
                $"MALE_OPPONENT={MaleOpponent}",
                $"FEMALE_TEAM={FemaleTeam}",
                $"FEMALE_OPPONENT={FemaleOpponent}"
            };

            foreach (var kvp in FavoritesByTeam)
                lines.Add($"PLAYERS_{kvp.Key}={string.Join(",", kvp.Value)}");

            File.WriteAllLines(_path, lines);
        }
    }
}