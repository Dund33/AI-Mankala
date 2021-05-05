using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI.Models;

namespace AI.GameEngine
{
    abstract class Algorithm
    {
        internal class Node
        {
            public Node? Parent { get; set; }
            public List<Node>? Children { get; set; }
            public int Value { get; set; }
            public State State { get; set; }
            public (int, int) Selection { get; set; }
        }
        internal record Tree(Node Root);

        public abstract int? GetMove(State state, int depth, bool player1);
    }
}
