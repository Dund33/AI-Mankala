using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Models
{
    public class State
    {
        public ImmutableArray<ImmutableArray<int>> HolesState { get; set; }
        public ImmutableArray<int> Wells { get; set; }
        public int Player { get; set; }
        public bool GameOver { get; set; }
    }
}
