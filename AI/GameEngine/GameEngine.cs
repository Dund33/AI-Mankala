using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI.Models;

namespace AI.GameEngine
{
    class GameEngine
    {
        public static bool IsValidMove(Move move)
        {
            var (x, y) = move.Selection;
            var stonesLeft = move.OldState.HolesState[x][y];
            var nonEmpty = stonesLeft > 0;
            var validPlayer = y == move.OldState.Player;

            return nonEmpty && validPlayer;
        }

        public static State MakeMove(Move move)
        {
            var (x, y) = move.Selection;
            var stonesLeft = move.OldState.HolesState[x][y];
            var resIntArrMut = move.OldState.HolesState.Select(r => r.ToArray()).ToArray();
            var goingRight = y == 1;
            var wellsMut = move.OldState.Wells.ToArray();
            resIntArrMut[x][y] = 0;
            var player = move.OldState.Player;
            while (stonesLeft > 0)
            {
                x += goingRight ? 1 : -1;
                switch (x)
                {
                    case < 0:
                    {
                        if (player == 0)
                            wellsMut[0]++;
                        y = 1 - y;
                        goingRight = !goingRight;
                        break;
                    }
                    case > 5:
                    {
                        if (player == 1)
                            wellsMut[1]++;
                        y = 1 - y;
                        goingRight = !goingRight;
                        break;
                    }
                    default:
                        resIntArrMut[x][y]++;
                        break;
                }
                stonesLeft--;
            }

            return new State
            {
                HolesState = resIntArrMut.Select(s => s.ToImmutableArray()).ToImmutableArray(),
                Wells = wellsMut.ToImmutableArray(),
                Player = 1-move.OldState.Player
            };
        }
    }
}
