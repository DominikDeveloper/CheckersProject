using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersApplication
{
    public class ChessField
    {
        public int x;
        public int y;

        public int Value; //1 - square, 2 - piece

        public ChessField()
        {
            Value = 0;
            x = 0;
            y = 0;
        }
    }
}
