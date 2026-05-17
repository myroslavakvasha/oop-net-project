using DataLayer.Models;
using DataLayer.Models.Rankings;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataLayer.Services
{
    public class DataService
    {
        private readonly ConfigService _config;
        private readonly SelectionService _selections;
        private readonly HttpClient _httpClient = new();
        private readonly string _jsonFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources", "Json");

        private string BaseUrl =>
            $"http://worldcup-vua.nullbit.hr/{(_selections.Gender == "male" ? "men" : "women")}";

        private string JsonPath
        {
            get
            {
                if (_selections.Gender == "male") return Path.Combine(_jsonFolder, "men");
                else return Path.Combine(_jsonFolder, "women");
            }
        }

        public DataService(ConfigService config, SelectionService selections)
        {
            _config = config;
            _selections = selections;
        }

        public async Task<List<Team>> GetTeamsAsync() 
            => _config.IsAPI
                ? await FetchAsync<List<Team>>($"{BaseUrl}/teams/results")
                : ReadJson<List<Team>>(Path.Combine(JsonPath, "results.json"));

        public async Task<List<Group>> GetGroupResultsAsync() 
            => _config.IsAPI
                ? await FetchAsync<List<Group>>($"{BaseUrl}/teams/group_results")
                : ReadJson<List<Group>>(Path.Combine(JsonPath, "group_results.json"));

        public async Task<List<Match>> GetMatchesAsync() 
            => _config.IsAPI
                ? await FetchAsync<List<Match>>($"{BaseUrl}/matches")
                : ReadJson<List<Match>>(Path.Combine(JsonPath, "matches.json"));

        public async Task<List<Match>> GetMatchesByTeamAsync(string fifaCode)
        {
            if (_config.IsAPI)
                return await FetchAsync<List<Match>>($"{BaseUrl}/matches/country?fifa_code={fifaCode}");

            var all = ReadJson<List<Match>>(Path.Combine(JsonPath, "matches.json"));
            return all
                .Where(m => m.HomeTeam.Code.Equals(fifaCode, StringComparison.OrdinalIgnoreCase)
                         || m.AwayTeam.Code.Equals(fifaCode, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public async Task<(List<PlayerRankingModel> players, List<MatchRankingModel> matches)> GetTeamRankingsAsync(string fifaCode)
        {
            var teamMatches = await GetMatchesByTeamAsync(fifaCode);

            var playerMap = new Dictionary<string, PlayerRankingModel>();
            var matchRankings = new List<MatchRankingModel>();
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string currentGender = _selections.Gender?.ToLower() ?? "male";

            foreach (var match in teamMatches)
            {
                matchRankings.Add(new MatchRankingModel
                {
                    HostTeam = match.HomeTeam.Country,
                    GuestTeam = match.AwayTeam.Country,
                    Location = match.Venue,
                    Attendance = match.Attendance
                });

                bool isHome = match.HomeTeam.Code.Equals(fifaCode, StringComparison.OrdinalIgnoreCase);

                var statistics = isHome ? match.HomeTeamStatistics : match.AwayTeamStatistics;
                var events = isHome ? match.HomeTeamEvents : match.AwayTeamEvents;

                if (statistics != null)
                {
                    var matchSquad = statistics.StartingEleven.Concat(statistics.Substitutes).ToList();

                    foreach (var p in matchSquad)
                    {
                        if (!playerMap.ContainsKey(p.Name))
                        {
                            var customImg = Path.Combine(baseDir, "Resources", "img", "players", $"{currentGender}_{fifaCode}_{p.ShirtNumber}.png");
                            var defaultImg = Path.Combine(baseDir, "Resources", "img", "players", "default.png");

                            playerMap[p.Name] = new PlayerRankingModel
                            {
                                Name = p.Name,
                                ShirtNumber = p.ShirtNumber,
                                Position = p.Position.ToString(),
                                ImagePath = File.Exists(customImg) ? customImg : defaultImg,
                                Appearances = 0,
                                Goals = 0,
                                YellowCards = 0
                            };
                        }

                        playerMap[p.Name].Appearances++;
                    }
                }

                if (events != null)
                {
                    foreach (var ev in events)
                    {
                        if (playerMap.ContainsKey(ev.Player))
                        {
                            string eventStr = ev.TypeOfEvent.ToString().ToLower();

                            if (eventStr.Contains("goal"))
                            {
                                playerMap[ev.Player].Goals++;
                            }
                            else if (eventStr.Contains("yellow"))
                            {
                                playerMap[ev.Player].YellowCards++;
                            }
                        }
                    }
                }
            }

            var sortedPlayers = playerMap.Values.ToList();
            sortedPlayers.Sort();

            matchRankings.Sort();

            return (sortedPlayers, matchRankings);
        }

        private async Task<T> FetchAsync<T>(string url)
        {
            var json = await _httpClient.GetStringAsync(url);
            return JsonSerializer.Deserialize<T>(json)
                   ?? throw new InvalidOperationException($"Null result from {url}");
        }

        private T ReadJson<T>(string fileName)
        {
            var path = Path.Combine(_jsonFolder, fileName);
            if (!File.Exists(path))
                throw new FileNotFoundException($"Local JSON not found: {path}");

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json)
                   ?? throw new InvalidOperationException($"Null result from {fileName}");
        }
    }
}
