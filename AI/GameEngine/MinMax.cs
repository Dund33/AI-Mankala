using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AI.Models;

namespace AI.GameEngine
{
    internal class Node
    {
        public Node? Parent { get; set; }
        public List<Node>? Children { get; set; }
        public int Value { get; set; }
        public State State { get; set; }
        public (int,int) Selection { get; set; }
    }

    internal record Tree(Node Root);

    internal class MinMax
    {
        private static int Evaluate(State state)
        {
            return state.Wells[0] - state.Wells[1];
        }

        public static int BuildTree(State state, int dieepte)
        {
            List<Node> leaves = new List<Node>();

            void GetLeaves(Node node, int depth, bool bot)
            {
                if (depth == 0)
                {
                    leaves.Add(node);
                    return;
                }

                node.Children = new List<int> {0, 1, 2, 3, 4, 5}.Select(index =>
                    new Node
                    {
                        State = GameEngine.MakeMove(new Move {OldState = node.State, Selection = bot ? (index, 1): (index, 0)}),
                        Value = Evaluate(node.State),
                        Selection = bot ? (index, 1) : (index, 0),
                        Parent = node
                    }
                ).ToList();
                node.Children.Where(n=> GameEngine.IsValidMove(node.State, n.Selection)).ToList().ForEach(n => GetLeaves(n, depth - 1, !bot));
            }


            var node = new Node()
            {
                Children = null,
                Parent = null,
                State = state,
                Value = Evaluate(state)
            };
            GetLeaves(node,dieepte, true);

            var max = leaves.OrderByDescending(leaf => leaf.Value).FirstOrDefault();
            if (max == null)
                return -1;
            var result = new List<int>();

            while (max.Parent.Parent != null)
            {
                max = max.Parent;
            }

            return max.Selection.Item1;
        }
    }
}