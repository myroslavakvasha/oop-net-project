using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models.Enums
{
    public enum TypeOfEvent
    {
        [EnumMember(Value = "goal")]
        Goal,

        [EnumMember(Value = "goal-own")]
        GoalOwn,

        [EnumMember(Value = "goal-penalty")]
        GoalPenalty,

        [EnumMember(Value = "red-card")]
        RedCard,

        [EnumMember(Value = "substitution-in")]
        SubstitutionIn,

        [EnumMember(Value = "substitution-out")]
        SubstitutionOut,

        [EnumMember(Value = "yellow-card")]
        YellowCard,

        [EnumMember(Value = "yellow-card-second")]
        YellowCardSecond
    }
}
