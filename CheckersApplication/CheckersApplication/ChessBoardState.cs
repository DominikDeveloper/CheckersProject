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
                    if (fields[i, j].Value == (int)Player.WhiteMen)
                        pieces.Add(new CheckersPiece { Pos = new Point(j, i), Player = Player.WhiteMen });
                    else if (fields[i, j].Value == (int)Player.BlackMen)
                        pieces.Add(new CheckersPiece { Pos = new Point(j, i), Player = Player.BlackMen });
                }
            }
        }

        public void Clear()
        {
            pieces.Clear();
        }

    }
}
