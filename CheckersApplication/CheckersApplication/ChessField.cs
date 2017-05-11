using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersApplication
{
    public class ChessField
    {
        private Point[] points = new Point[4];
        public int Value; 

        public ChessField(Point[] points)
        {
            Value = 0;
            this.points = (Point[])points.Clone();
        }

        public static ChessField[,] GetChessFields(Point[] rectangle, ref Image<Bgr, Byte> img)
        {
            int width = rectangle[1].X - rectangle[0].X;
            int height = rectangle[3].Y - rectangle[0].Y;
            int fieldWidht = width / 8;
            int fieldHeight = height / 8;
            Point[] field = new Point[4];
            ChessField[,] fields = new ChessField[8,8];
            for(int i = 0; i < 8; i++)
            {
                field[0].Y = rectangle[0].Y + i * fieldHeight;
                field[1].Y = rectangle[0].Y + i * fieldHeight;
                field[2].Y = rectangle[0].Y + i * fieldHeight + fieldHeight;
                field[3].Y = rectangle[0].Y + i * fieldHeight + fieldHeight;
                for (int j = 0; j < 8; j++)
                {
                    field[0].X = rectangle[0].X + j * fieldWidht;
                    field[1].X = rectangle[0].X + j * fieldWidht + fieldWidht;
                    field[2].X = rectangle[0].X + j * fieldWidht + fieldWidht;
                    field[3].X = rectangle[0].X + j * fieldWidht;
                    fields[i, j] = new ChessField(field);
                    img.Draw(field, new Bgr(Color.Green), 2);
                }
            }
            return fields;            
        }

        public static void Pons(ChessField[,] fields, CircleF[] circles)
        {
            foreach (CircleF circle in circles)
            {
                foreach (ChessField field in fields)
                {
                    if(circle.Center.X>field.points[0].X && circle.Center.X < field.points[1].X && circle.Center.Y > field.points[0].Y && circle.Center.Y < field.points[3].Y)
                        field.Value = 2;
                }
            }
        }

    }
}
