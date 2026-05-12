using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataLayer.Models.SubModels
{
    public class MatchTeam
    {
        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("goals")]
        public int Goals { get; set; }

        [JsonPropertyName("penalties")]
        public int Penalties { get; set; }
    }
}
