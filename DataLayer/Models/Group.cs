using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class Group
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("letter")]
        public string Letter { get; set; } = string.Empty;

        [JsonPropertyName("ordered_teams")]
        public List<Team> OrderedTeams { get; set; } = new();

        public override bool Equals(object? obj)
        {
            return obj is Group group &&
                   Id == group.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
