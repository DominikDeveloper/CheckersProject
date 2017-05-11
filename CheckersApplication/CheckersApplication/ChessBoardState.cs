using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersApplication
{

    class ChessBoardState
    {
        public const int empty = 0;
        public ObservableCollection<CheckersPiece> pieces = new ObservableCollection<CheckersPiece>();

        public void AddPieces(ChessField[,] fields)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (fields[i, j].Value == 1)
                        pieces.Add(new CheckersPiece { Pos = new System.Drawing.Point(i, j), Player = Player.White });
                    else if (fields[i, j].Value == 2)
                        pieces.Add(new CheckersPiece { Pos = new System.Drawing.Point(i, j), Player = Player.Black });
                }
            }
        }

    }
}
