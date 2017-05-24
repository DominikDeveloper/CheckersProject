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
        public Point[] points { get; } = new Point[4];
        public int Value; 

        public ChessField(Point[] points)
        {
            Value = 0;
            this.points = (Point[])points.Clone();
        }

        public static ChessField[,] GetChessFields(Point[] rectangle)
        {
            Point[] rectangle2 = rectangle;
            IEnumerable<Point> query = rectangle2.OrderBy(r => r.X);
            if (query.ElementAt<Point>(0).Y < query.ElementAt<Point>(1).Y)
            {
                rectangle[0] = query.ElementAt<Point>(0);
                rectangle[2] = query.ElementAt<Point>(1);
            }
            else
            {
                rectangle[0] = query.ElementAt<Point>(1);
                rectangle[2] = query.ElementAt<Point>(0);
            }

            if (query.ElementAt<Point>(2).Y < query.ElementAt<Point>(3).Y)
            {
                rectangle[1] = query.ElementAt<Point>(2);
                rectangle[3] = query.ElementAt<Point>(3);
            }
            else
            {
                rectangle[1] = query.ElementAt<Point>(3);
                rectangle[3] = query.ElementAt<Point>(2);
            }

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
