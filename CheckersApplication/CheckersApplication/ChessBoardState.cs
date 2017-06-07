using System.Collections.ObjectModel;
using System.Drawing;

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
                        pieces.Add(new CheckersPiece { Pos = new Point(j, i), Player = Player.White });
                    else if (fields[i, j].Value == 2)
                        pieces.Add(new CheckersPiece { Pos = new Point(j, i), Player = Player.White });
                }
            }
        }

        public void Clear()
        {
            pieces.Clear();
        }

    }
}
