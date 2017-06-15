//MovesJumps.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersApplication
{
    public class MovesBody
    {
        public MovesBody() { }
    }

    public partial class MovesJumps
    {
        public static List<ChessField[,]> move_matrix_buffer_white;
        public static List<ChessField[,]> move_matrix_buffer_black;

        public static int indexxx = 0;
        public static int indexxxBlack = 0;
    }
}