using DataLayer.Converters;
using DataLayer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataLayer.Models
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

        public override bool Equals(object? obj)
        {
            return obj is Player player &&
                   Name == player.Name &&
                   ShirtNumber == player.ShirtNumber;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, ShirtNumber);
        }
    }
}
