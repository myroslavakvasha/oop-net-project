using DataLayer.Converters;
using DataLayer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataLayer.Models.SubModels
{
    public class Player
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("captain")]
        public bool Captain { get; set; }

        [JsonPropertyName("shirt_number")]
        public int ShirtNumber { get; set; }

        [JsonPropertyName("position")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Position Position { get; set; }
    }
}
