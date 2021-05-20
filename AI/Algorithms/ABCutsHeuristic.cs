﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using AI.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AI.Algorithms
{
    class ABCutsHeuristic: Algorithm
    {
        protected override int Evaluate(State state)
        {
            var penalty = state.Player == 0 ? state.HolesState[1].Sum() : state.HolesState[0].Sum();
            return base.Evaluate(state) - penalty;
        }

        public override int? GetMove(State state, int depth, bool player1)
        {
            int AlphaBetaRec(Node node, int depthRec, bool player1Rec, int alpha, int beta)
            {
                if (depthRec == 0)
                    return Evaluate(node.State);
                if (node.State.GameOver)
                {
                    return player1Rec switch
                    {
                        true => int.MaxValue,
                        false => int.MinValue
                    };
                }
                if (player1Rec)
                {
                    var value = int.MinValue;
                    foreach (var child in node.Children)
                    {
                        var childValue = AlphaBetaRec(child, depthRec - 1, !player1Rec, alpha, beta);
                        
                        value = Math.Max(value, childValue);
                        alpha = Math.Max(alpha, value);
                        if (childValue == int.MaxValue)
                            break;
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
                        var childValue = AlphaBetaRec(child, depthRec - 1, !player1Rec, alpha, beta);
                       
                        value = Math.Min(value, childValue);
                        beta = Math.Min(beta, value);
                        if (childValue == int.MinValue)
                            break;
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
