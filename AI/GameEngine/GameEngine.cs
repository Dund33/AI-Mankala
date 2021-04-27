using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI.Models;

namespace AI.GameEngine
{
    class GameEngine
    {
        private static bool IsValidTransition(Move state)
        {
            throw new NotImplementedException();
        }

        public static bool MakeMove(Move move)
        {
            if (!IsValidTransition(move))
                return false;
            throw new NotImplementedException();

        }
    }
}
