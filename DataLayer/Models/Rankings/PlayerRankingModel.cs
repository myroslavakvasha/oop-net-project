using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models.Rankings
{
    public class PlayerRankingModel : IComparable<PlayerRankingModel>
    {
        public string Name { get; set; } = string.Empty;
        public int ShirtNumber { get; set; }
        public string Position { get; set; } = string.Empty;
        public int Goals { get; set; }
        public int YellowCards { get; set; }
        public int Appearances { get; set; }
        public string ImagePath { get; set; } = string.Empty;

        
        public int CompareTo(PlayerRankingModel? other)
        {
            if (other == null) return 1;
            int goalCompare = other.Goals.CompareTo(this.Goals);
            if (goalCompare != 0) return goalCompare;
            return other.YellowCards.CompareTo(this.YellowCards);
        }
    }
}