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
        private int[,] matrix = new int[8, 8];
        public ObservableCollection<CheckersPiece> pieces = new ObservableCollection<CheckersPiece>();

        public void TestData()
        {
            Random rnd = new Random();
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    int state = rnd.Next(0, 3);
                    if (state == 1)
                        matrix[i, j] = 1;
                    else if (state == 2)
                        matrix[i, j] = 2;
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

    }
}
