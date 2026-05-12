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
    public class TeamEvent
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type_of_event")]
        [JsonConverter(typeof(EnumMemberConverter<TypeOfEvent>))]
        public TypeOfEvent TypeOfEvent { get; set; }

        [JsonPropertyName("player")]
        public string Player { get; set; } = string.Empty;

        [JsonPropertyName("time")]
        public string Time { get; set; } = string.Empty;
    }
}
