using AI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI.GameEngine;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AI.Algorithms
{
    class ABCuts : Algorithm
    {

        public override int? GetMove(State state, int depth, bool player1)
        {
            int AlphaBetaRec(Node node, int depthRec, bool player1Rec, int alpha, int beta)
            {
                if(depthRec == 0 || node.State.GameOver)
                    return Evaluate(node.State);
                if (player1Rec)
                {
                    var value = int.MinValue;
                    foreach (var child in node.Children)
                    {
                        value = Math.Max(value, AlphaBetaRec(child, depthRec - 1, !player1Rec, alpha, beta));
                        alpha = Math.Max(alpha, value);
                        if (alpha >= beta)
                            break;
                    }

                    node.Value = value;
                    return value;
                }
                else
                {
                    var value = int.MaxValue;
                    foreach (var child in node.Children)
                    {
                        value = Math.Min(value, AlphaBetaRec(child, depthRec-1,!player1Rec, alpha, beta));
                        beta = Math.Min(beta, value);
                        if (beta <= alpha)
                            break;
                    }

                    node.Value = value;
                    return value;
                }
            }
            var root = BuildTree(state, depth);
            AlphaBetaRec(root, depth, player1, int.MinValue, int.MaxValue);
            var bestChild = root.Children.OrderByDescending(child => child.Value).FirstOrDefault();
            return bestChild?.Selection.Item1;
        }
    }
}
