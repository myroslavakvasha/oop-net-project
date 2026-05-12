using DataLayer.Converters;
using DataLayer.Models.Enums;
using DataLayer.Models.SubModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Timers;

namespace DataLayer.Models
{
    public class Match
    {
        [JsonPropertyName("venue")]
        public string Venue { get; set; } = string.Empty;

        [JsonPropertyName("location")]
        public string Location { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        [JsonConverter(typeof(EnumMemberConverter<MatchStatus>))]
        public MatchStatus Status { get; set; }

        [JsonPropertyName("time")]
        [JsonConverter(typeof(EnumMemberConverter<MatchTime>))]
        public MatchTime Time { get; set; }

        [JsonPropertyName("fifa_id")]
        [JsonConverter(typeof(ParseStringLongConverter))]
        public long FifaId { get; set; }

        [JsonPropertyName("weather")]
        public Weather Weather { get; set; }

        [JsonPropertyName("attendance")]
        [JsonConverter(typeof(ParseStringLongConverter))]
        public long Attendance { get; set; }

        [JsonPropertyName("officials")]
        public List<string> Officials { get; set; } = new();

        [JsonPropertyName("stage_name")]
        [JsonConverter(typeof(EnumMemberConverter<StageName>))]
        public StageName StageName { get; set; }

        [JsonPropertyName("home_team_country")]
        public string HomeTeamCountry { get; set; } = string.Empty;

        [JsonPropertyName("away_team_country")]
        public string AwayTeamCountry { get; set; } = string.Empty;

        [JsonPropertyName("datetime")]
        public DateTimeOffset Datetime { get; set; }

        [JsonPropertyName("winner")]
        public string Winner { get; set; } = string.Empty;

        [JsonPropertyName("winner_code")]
        public string WinnerCode { get; set; } = string.Empty;

        [JsonPropertyName("home_team")]
        public MatchTeam HomeTeam { get; set; }

        [JsonPropertyName("away_team")]
        public MatchTeam AwayTeam { get; set; }

        [JsonPropertyName("home_team_events")]
        public List<TeamEvent> HomeTeamEvents { get; set; } = new();

        [JsonPropertyName("away_team_events")]
        public List<TeamEvent> AwayTeamEvents { get; set; } = new();

        [JsonPropertyName("home_team_statistics")]
        public TeamStatistics HomeTeamStatistics { get; set; }

        [JsonPropertyName("away_team_statistics")]
        public TeamStatistics AwayTeamStatistics { get; set; }

        [JsonPropertyName("last_event_update_at")]
        public DateTimeOffset LastEventUpdateAt { get; set; }

        [JsonPropertyName("last_score_update_at")]
        public DateTimeOffset? LastScoreUpdateAt { get; set; }
    }
}
