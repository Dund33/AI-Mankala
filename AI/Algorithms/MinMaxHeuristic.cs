using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AI.Algorithms
{
    class MinMaxHeuristic: Algorithm
    {
        protected override int Evaluate(State state)
        {
            var penalty = state.Player == 0 ? state.HolesState[1].Sum() : state.HolesState[0].Sum();
            return base.Evaluate(state) - penalty;
        }

        public override int? GetMove(State state, int dieepte, bool player1)
        {
            int MinMaxRec(Node node, int depth, bool player1rec)
            {
                if (depth == 0)
                    return Evaluate(node.State);
                if (node.State.GameOver)
                {
                    return player1rec switch
                    {
                        true => int.MaxValue,
                        false => int.MinValue
                    };
                }
                if (!player1rec)
                {
                    var value = int.MinValue;
                    foreach(var child in node.Children)
                    {
                        var childValue = MinMaxRec(child, depth - 1, !player1rec);
                        value = Math.Max(value, childValue);
                        if (childValue == int.MaxValue)
                            break;
                    }
                    node.Value = value;
                    return value;
                }
                else
                {
                    var value = int.MaxValue;
                    foreach(var child in node.Children)
                    {
                        var childValue = MinMaxRec(child, depth - 1, !player1rec);
                        value = Math.Min(value, childValue);
                        if (childValue == int.MinValue)
                            break;
                    }
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
