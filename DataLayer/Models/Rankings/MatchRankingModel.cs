using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models.Rankings
{
    public class MatchRankingModel : IComparable<MatchRankingModel>
    {
        public string HostTeam { get; set; } = string.Empty;
        public string GuestTeam { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public long Attendance { get; set; }

        
        public int CompareTo(MatchRankingModel? other)
        {
            if (other == null) return 1;
            return other.Attendance.CompareTo(this.Attendance);
        }
    }
}
