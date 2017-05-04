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
        private int[,] matrix = new int[8, 8];
        public ObservableCollection<CheckersPiece> pieces = new ObservableCollection<CheckersPiece>();

        public void TestData()
        {
            Random rnd = new Random();
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    int state = rnd.Next(1, 6);
                    if (state == (int)Player.White)
                        matrix[i, j] = (int)Player.White;
                    else if (state == (int)Player.Black)
                        matrix[i, j] = (int)Player.Black;
                }
            }
            matrixToPieces();
        }

        public void matrixToPieces()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (matrix[i, j] == 1)
                        pieces.Add(new CheckersPiece { Pos = new System.Drawing.Point(i, j), Player = Player.White });
                    else if (matrix[i, j] == 2)
                        pieces.Add(new CheckersPiece { Pos = new System.Drawing.Point(i, j), Player = Player.Black });
                }
            }
        }

        public void matrixToPieces_tmp(int[,] ChessboardArray)
        {
            pieces.Clear();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (ChessboardArray[i, j] == 2)
                        pieces.Add(new CheckersPiece { Pos = new System.Drawing.Point(j, i), Player = Player.White });
                }
            }
        }

    }
}
