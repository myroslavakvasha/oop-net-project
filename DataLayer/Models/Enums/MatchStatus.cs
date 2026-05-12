using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models.Enums
{
    public enum MatchStatus
    {
        [EnumMember(Value = "completed")]
        Completed
    }
}
