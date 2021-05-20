using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI.Models;
using AI.GameEngine;

namespace AI.Algorithms
{
    abstract class Algorithm
    {
        internal class Node
        {
            public Node? Parent { get; set; }
            public List<Node>? Children { get; set; }
            public int Value { get; set; }
            public State? State { get; set; }
            public (int, int) Selection { get; set; }
        }
        internal record Tree(Node Root);

        protected virtual int Evaluate(State state)
        {
            return state.Wells[1] - state.Wells[0];
        }

        protected Node BuildTree(State state, int dieepte)
        {
            List<Node> leaves = new();

            void BuildTreeRec(Node node, int depth, bool bot)
            {
                if (depth == 0)
                {
                    leaves.Add(node);
                    return;
                }

                node.Children = new List<int> { 0, 1, 2, 3, 4, 5 }.Select(index =>
                    new Node
                    {
                        State = GameEngine.GameEngine.MakeMove(new Move
                            { OldState = node.State, Selection = bot ? (index, 1) : (index, 0) }),
                        Selection = bot ? (index, 1) : (index, 0),
                        Parent = node
                    }
                ).Where(n => GameEngine.GameEngine.IsValidMove(node.State, n.Selection)).ToList();
                node.Children.ToList()
                    .ForEach(n => BuildTreeRec(n, depth - 1, !bot));
            }

            var root = new Node
            {
                Children = null,
                Parent = null,
                State = state,
            };
            BuildTreeRec(root, dieepte, true);

            return root;
        }

        public abstract int? GetMove(State state, int depth, bool player1);
    }
}
