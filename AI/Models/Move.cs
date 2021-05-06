using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Models
{
    public class Move
    {
        public State? OldState { get; set; }
        public (int,int) Selection { get; set; }
    }
}
