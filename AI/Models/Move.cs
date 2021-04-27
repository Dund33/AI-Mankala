using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Models
{
    public class Move
    {
        public int[][] State1 { get; set; }
        public int[][] State2 { get; set; }
        public int Selection { get; set; }

        public Move(int[][] state1, int[][] state2)
        {
            State1 = state1.Select(row => (int[]) row.Clone()).ToArray();
            State2 = state2.Select(row => (int[]) row.Clone()).ToArray();
        }
    }
}
