using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class Team
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("alternate_name")]
        public string AlternateName { get; set; } = string.Empty;

        [JsonPropertyName("fifa_code")]
        public string FifaCode { get; set; } = string.Empty;

        [JsonPropertyName("group_id")]
        public int GroupId { get; set; }

        [JsonPropertyName("group_letter")]
        public string GroupLetter { get; set; } = string.Empty;

        // These fields appear in results.json and group_results.json
        [JsonPropertyName("wins")]
        public int Wins { get; set; }

        [JsonPropertyName("draws")]
        public int Draws { get; set; }

        [JsonPropertyName("losses")]
        public int Losses { get; set; }

        [JsonPropertyName("games_played")]
        public int GamesPlayed { get; set; }

        [JsonPropertyName("points")]
        public int Points { get; set; }

        [JsonPropertyName("goals_for")]
        public int GoalsFor { get; set; }

        [JsonPropertyName("goals_against")]
        public int GoalsAgainst { get; set; }

        [JsonPropertyName("goal_differential")]
        public int GoalDifferential { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Team team &&
                   Id == team.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString() => $"{Country} ({FifaCode})";
    }
}
