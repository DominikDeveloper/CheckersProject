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

        public ChessField(PointF[] points)
        {
            Value = 0;
            Point[] pts = new Point[points.Length];
            for (int i = 0; i < points.Length; i++)
                pts[i] = Point.Round(points[i]);
            this.points = (Point[])pts.Clone();
        }

        private static int DefOfWidthSize(Point[] vertices)
        {
            //definition of width of board

            var compareForRight1 = Math.Max(vertices[0].X, vertices[1].X);
            var compareForRight2 = Math.Max(vertices[2].X, vertices[3].X);
            var extremeRight = Math.Max(compareForRight1, compareForRight2);
            var compareForLeft1 = Math.Min(vertices[0].X, vertices[1].X);
            var compareForLeft2 = Math.Min(vertices[2].X, vertices[3].X);
            var extremeLeft = Math.Min(compareForLeft1, compareForLeft2);

            return Math.Abs(extremeRight - extremeLeft);
            //
        }

        private static int DefOfHeightSize(Point[] vertices)
        {
            var compareForBottom1 = Math.Max(vertices[0].Y, vertices[1].Y);
            var compareForBottom2 = Math.Max(vertices[2].Y, vertices[3].Y);
            var extremeDown = Math.Max(compareForBottom1, compareForBottom2);
            var compareForTop1 = Math.Min(vertices[0].Y, vertices[1].Y);
            var compareForTop2 = Math.Min(vertices[2].Y, vertices[3].Y);
            var extremeUp = Math.Min(compareForTop1, compareForTop2);

            return Math.Abs(extremeDown - extremeUp);
        }

        private static int DefOfIndexOfLeftTopVertice(Point[] vertices, ref List<float> list, ref List<float> list_copy)
        {
            //finding index of left-top vertice
            list_copy.Sort();
            int j = -1;
            do
            {
                j++;
            } while (list_copy.ElementAt(0) != list.ElementAt(j));
            int k = -1;
            do
            {
                k++;
            } while (list_copy.ElementAt(1) != list.ElementAt(k));

            int index = 0;
            if (Math.Min(vertices[j].Y, vertices[k].Y) == vertices[j].Y)
                index = j;
            else
                index = k;

            return index;
            //
        }

        private static int DefOfIndexOfRightTopVertice(Point[] vertices, ref List<float> list, ref List<float> list1_copy)
        {
            list1_copy.Sort();
            int j1 = -1;
            do
            {
                j1++;
            } while (list1_copy.ElementAt(list1_copy.Count - 1) != list.ElementAt(j1));
            int k1 = -1;
            do
            {
                k1++;
            } while (list1_copy.ElementAt(list1_copy.Count - 2) != list.ElementAt(k1));

            int index1 = 0;
            if (Math.Min(vertices[j1].Y, vertices[k1].Y) == vertices[j1].Y)
                index1 = j1;
            else
                index1 = k1;

            return index1;
        }

        private static int DefOfLeftBottomPointX(Point[] vertices, int indexLeftTop, int indexRightTop)
        {
            var list2 = new List<PointF>();
            int element = 0;
            foreach (var item in vertices)
            {
                if (element != indexLeftTop && element != indexRightTop)
                    list2.Add(item);
                element++;
            }

            return Math.Min(
                Convert.ToInt32(list2.ElementAt(0).X), 
                Convert.ToInt32(list2.ElementAt(1).X));
        }

        public static ChessField[,] GetChessFields(Point[] vertices)
        {

            ChessField[,] fields = new ChessField[8, 8];

            int width_board = DefOfWidthSize(vertices);
            int height_board = DefOfHeightSize(vertices);
            //size one square
            var width_square = width_board / 8;
            var height_square = height_board / 8;
            //
            
            //for 0,0 square...
            //finding index of left-top vertice
            var list = new List<float> { vertices[0].X, vertices[1].X, vertices[2].X, vertices[3].X };
            var list_copy = new List<float> { vertices[0].X, vertices[1].X, vertices[2].X, vertices[3].X };

            int leftTopIndex = DefOfIndexOfLeftTopVertice(vertices, ref list, ref list_copy);
            //

            //finding index of right-top vertice
            var list1_copy = new List<float> { vertices[0].X, vertices[1].X, vertices[2].X, vertices[3].X };
            int rightTopIndex = DefOfIndexOfRightTopVertice(vertices, ref list, ref list1_copy);
            //

            //finding X-position of left-bottom vertice
            var LeftBottomPointX = DefOfLeftBottomPointX(vertices, leftTopIndex, rightTopIndex);
            //

            //till (horizontal)
            var level = vertices[rightTopIndex].Y - vertices[leftTopIndex].Y;
            float till_level_input = (float)level / 8;
            float current_till_horiz = 0;

            //till (vertical)
            var plumb = LeftBottomPointX - vertices[leftTopIndex].X;
            float till_plumb_input = (float)plumb / 8;
            float current_till_vert = 0;

            float offset_X = 0; float offset_Y = 0;
            int indexX = 0, indexY = 0;

            for (int i = 0; i < 64; i++)
            {
                System.Drawing.PointF[] pts = new System.Drawing.PointF[4];
                pts[0] = new System.Drawing.PointF(
                    vertices[leftTopIndex].X + offset_X + current_till_vert, vertices[leftTopIndex].Y + offset_Y + current_till_horiz);
                pts[1] = new System.Drawing.PointF(
                    vertices[leftTopIndex].X + offset_X + width_square + current_till_vert, vertices[leftTopIndex].Y + offset_Y + current_till_horiz);
                pts[2] = new System.Drawing.PointF(
                    vertices[leftTopIndex].X + offset_X + width_square + current_till_vert, vertices[leftTopIndex].Y + offset_Y + height_square + current_till_horiz);
                pts[3] = new System.Drawing.PointF(
                    vertices[leftTopIndex].X + offset_X + current_till_vert, vertices[leftTopIndex].Y + offset_Y + height_square + current_till_horiz);
                
                offset_X += width_square;
                current_till_horiz += till_level_input;

                fields[indexX, indexY] = new ChessField(pts);
                indexY++;
                if ((i + 1) % 8 == 0 && i != 0)
                {
                    indexY = 0;
                    indexX++;
                    offset_X = 0;
                    offset_Y += height_square;
                    current_till_horiz = 0;
                    current_till_vert += till_plumb_input;
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
