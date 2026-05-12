using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models.Enums
{
    public enum WeatherDescription
    {
        [EnumMember(Value = "Clear Night")]
        ClearNight,

        [EnumMember(Value = "Cloudy")]
        Cloudy,

        [EnumMember(Value = "Party Cloudy")]
        PartlyCloudy,

        [EnumMember(Value = "Partly Cloudy Night")]
        PartlyCloudyNight,

        [EnumMember(Value = "Sunny")]
        Sunny
    }
}
