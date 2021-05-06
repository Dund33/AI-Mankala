using System;
using System.Collections.Generic;
using System.Linq;
using AI.GameEngine;
using AI.Models;
using Avalonia.Layout;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AI.Algorithms
{
    internal class MinMax: Algorithm
    {
        
        public override int? GetMove(State state, int dieepte, bool player1)
        {
            int MinMaxRec(Node node, int depth, bool player1rec)
            {
                if (depth == 0 || node.State.GameOver)
                    return Evaluate(node.State);
                if (!player1rec)
                {
                    var value = int.MinValue;
                    node.Children?.ForEach(child =>
                    {
                        value = Math.Max(value, MinMaxRec(child, depth - 1, !player1rec));
                    });
                    node.Value = value;
                    return value;
                }
                else
                {
                    var value = int.MaxValue;
                    node.Children?.ForEach(child =>
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