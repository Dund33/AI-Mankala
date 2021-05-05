using System;
using System.Collections.Generic;
using System.Linq;
using AI.Models;
using Avalonia.Layout;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AI.GameEngine
{
    internal class MinMax: Algorithm
    {
        private static int Evaluate(State state)
        {
            return state.Wells[1] - state.Wells[0];
        }

        private static Node BuildTree(State state, int dieepte)
        {
            List<Node> leaves = new();

            void BuildTreeRec(Node node, int depth, bool bot)
            {
                if (depth == 0)
                {
                    leaves.Add(node);
                    return;
                }

                node.Children = new List<int> {0, 1, 2, 3, 4, 5}.Select(index =>
                    new Node
                    {
                        State = GameEngine.MakeMove(new Move
                            {OldState = node.State, Selection = bot ? (index, 1) : (index, 0)}),
                        Selection = bot ? (index, 1) : (index, 0),
                        Parent = node
                    }
                ).Where(n => GameEngine.IsValidMove(node.State, n.Selection)).ToList();
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

        public override int? GetMove(State state, int dieepte, bool player1)
        {
            static int MinMaxRec(Node node, int depth, bool player1rec)
            {
                if (depth == 0 || node.State.GameOver)
                    return Evaluate(node.State);
                if (!player1rec)
                {
                    var value = int.MinValue;
                    node.Children.ForEach(child =>
                    {
                        value = Math.Max(value, MinMaxRec(child, depth - 1, !player1rec));
                    });
                    node.Value = value;
                    return value;
                }
                else
                {
                    var value = int.MaxValue;
                    node.Children.ForEach(child =>
                    {
                        value = Math.Min(value, MinMaxRec(child, depth - 1,!player1rec));
                    });
                    node.Value = value;
                    return value;
                }
            }

            var root = BuildTree(state, dieepte);
            MinMaxRec(root, dieepte, player1);
            var bestChild = root.Children.OrderByDescending(child => child.Value).FirstOrDefault();
            return bestChild?.Selection.Item1;
        }
    }
}