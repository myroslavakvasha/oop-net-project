using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models.Enums
{
    public enum StageName
    {
        [EnumMember(Value = "First stage")]
        FirstStage,

        [EnumMember(Value = "Round of 16")]
        RoundOf16,

        [EnumMember(Value = "Quarter-finals")]
        QuarterFinals,

        [EnumMember(Value = "Semi-finals")]
        SemiFinals,

        [EnumMember(Value = "Play-off for third place")]
        PlayOffForThirdPlace,

        [EnumMember(Value = "Final")]
        Final
    }
}
