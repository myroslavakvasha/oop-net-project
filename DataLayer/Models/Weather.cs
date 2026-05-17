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
    public class Weather
    {
        [JsonPropertyName("humidity")]
        [JsonConverter(typeof(ParseStringIntConverter))]
        public int Humidity { get; set; }

        [JsonPropertyName("temp_celsius")]
        [JsonConverter(typeof(ParseStringIntConverter))]
        public int TempCelsius { get; set; }

        [JsonPropertyName("temp_farenheit")]
        [JsonConverter(typeof(ParseStringIntConverter))]
        public int TempFarenheit { get; set; }

        [JsonPropertyName("wind_speed")]
        [JsonConverter(typeof(ParseStringIntConverter))]
        public int WindSpeed { get; set; }

        [JsonPropertyName("description")]
        [JsonConverter(typeof(EnumMemberConverter<WeatherDescription>))]
        public WeatherDescription Description { get; set; }
    }
}
