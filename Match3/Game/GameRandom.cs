using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Game
{
    public static class GameRandom
    {
        private static Random random = null;

        public static Random Random 
        {
            get
            {
                if (random == null)
                {
                    random = new Random();
                }
                return random;
            }
        }
    }
}
