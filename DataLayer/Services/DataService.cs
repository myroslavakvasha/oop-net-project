using DataLayer.Models;
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

        public DataService(ConfigService config, SelectionService selections)
        {
            _config = config;
            _selections = selections;
        }

        public async Task<List<Team>> GetTeamsAsync() 
            => _config.IsAPI
                ? await FetchAsync<List<Team>>($"{BaseUrl}/teams/results")
                : ReadJson<List<Team>>("results.json");

        public async Task<List<Group>> GetGroupResultsAsync() 
            => _config.IsAPI
                ? await FetchAsync<List<Group>>($"{BaseUrl}/teams/group_results")
                : ReadJson<List<Group>>("group_results.json");

        public async Task<List<Match>> GetMatchesAsync() 
            => _config.IsAPI
                ? await FetchAsync<List<Match>>($"{BaseUrl}/matches")
                : ReadJson<List<Match>>("matches.json");

        public async Task<List<Match>> GetMatchesByTeamAsync(string fifaCode)
        {
            if (_config.IsAPI)
                return await FetchAsync<List<Match>>($"{BaseUrl}/matches/country?fifa_code={fifaCode}");

            var all = ReadJson<List<Match>>("matches.json");
            return all
                .Where(m => m.HomeTeam.Code.Equals(fifaCode, StringComparison.OrdinalIgnoreCase)
                         || m.AwayTeam.Code.Equals(fifaCode, StringComparison.OrdinalIgnoreCase))
                .ToList();
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
