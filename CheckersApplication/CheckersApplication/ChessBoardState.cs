using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace CheckersApplication
{

    class ChessBoardState
    {
        public const int empty = 0;
        public ObservableCollection<CheckersPiece> piecesObservable = new ObservableCollection<CheckersPiece>();
        public List<List<CheckersPiece>> history = new List<List<CheckersPiece>>();

        public void AddPieces(ChessField[,] fields)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (fields[i, j].Value == (int)Player.WhiteMen)
                        piecesObservable.Add(new CheckersPiece { Pos = new Point(j, i), Player = Player.WhiteMen });
                    else if (fields[i, j].Value == (int)Player.BlackMen)
                        piecesObservable.Add(new CheckersPiece { Pos = new Point(j, i), Player = Player.BlackMen });
                }
            }
        }

        public void Clear()
        {
            piecesObservable.Clear();
        }

    }
}
