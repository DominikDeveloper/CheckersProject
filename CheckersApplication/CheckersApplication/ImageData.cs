using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using System.Windows;
using System.Drawing;
using Emgu.CV.CvEnum;

namespace CheckersApplication
{
    public class ImageData
    {
        Image<Bgr, byte> image;
        System.Drawing.Size patternSize;
        bool ifPatternWasFound;
        bool ifGettingCorners;

        public ImageData()
        {
            this.image = null;
            this.patternSize = new System.Drawing.Size(0,0);
            this.ifPatternWasFound = false;
            this.ifGettingCorners = false;
        }

        public ImageData(Image<Bgr, byte> _image, UInt16 width, UInt16 height)
        {
            this.image = _image;
            this.patternSize = new System.Drawing.Size(width-1, height-1); //-1 for chessboard
            this.ifPatternWasFound = false;
        }

        public ImageData(IImage _image, UInt16 width, UInt16 height)
        {
            Bitmap bmp = new Bitmap(_image.Bitmap);
            this.image = new Image<Bgr, Byte>(bmp);

            this.patternSize = new System.Drawing.Size(width - 1, height - 1); //-1 for chessboard
            this.ifPatternWasFound = false;
        }

        public bool IfPatternFound()
        {
            if (!ifGettingCorners)
            {
                GetInternalCorners();
            }

            return this.ifPatternWasFound;
        }

        public Image<Bgr, byte> GetSourceImage()
        {
            return image;
        }

        public VectorOfPointF GetInternalCorners()
        {
            ifGettingCorners = true;

            Image<Bgr, byte> outputImage = image.Copy();

            var cornerPoints = new VectorOfPointF();
            this.ifPatternWasFound = CvInvoke.FindChessboardCorners(
                outputImage, patternSize, cornerPoints, Emgu.CV.CvEnum.CalibCbType.AdaptiveThresh | Emgu.CV.CvEnum.CalibCbType.FilterQuads);

            return cornerPoints;
        }

        public IImage GetDrawingCorners(VectorOfPointF cornerPoints)
        {
            IImage outputImage = (IImage)image.Clone();

            try
            {
                CvInvoke.DrawChessboardCorners(outputImage, patternSize, cornerPoints, ifGettingCorners);
                return outputImage;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            return null;
        }

        public CircleF[] GetCirclesPositions()
        {
            Detection detect = new Detection();

            UMat uimage = detect.ConvertClearImage(image);

            //PARAMETERS
            double cannyThreshold = 140.0;
            double circleAccumulatorThreshold = 40.0;
            double dp = 2.0;
            double minDist = 30.0;
            int minRadius = 5;
            int maxRadius = 30;
            //END PARAMETERS

            CircleF[] circles = null;

            try
            {
                circles = CvInvoke.HoughCircles(
                    uimage, HoughType.Gradient, dp, minDist, cannyThreshold, circleAccumulatorThreshold, minRadius, maxRadius);

                return circles;
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }

        public IImage GetDrawingCircles(CircleF[] circles, int thickness = 3)
        {
            Image<Bgr,byte> outputImage = image.Copy();

            foreach (CircleF circle in circles)
                outputImage.Draw(circle, new Bgr(Color.Blue), thickness);

            return outputImage;
        }

        public List<RotatedRect> GetRectangles(bool ShowCanny = true)
        {
            bool alreadyExists = false;

            Image<Bgr, Byte> img = image.Copy();

            Image<Bgr, Byte> img3 = img.CopyBlank();

            List<RotatedRect> rectangles = new List<RotatedRect>(); //a box is a rotated rectangle

            int contourAreaMin = 10000;//300
            int contourAreaMax = 500000;
            int angleMin = 75;
            int angleMax = 105;

            UMat cannyEdges = GetCanny();
            if (ShowCanny)
                CvInvoke.Imshow("Canny edges", cannyEdges);

            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                for (int i = 0; i < contours.Size; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    using (VectorOfPoint approxContour = new VectorOfPoint())
                    {
                        CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
                        double contourArea = CvInvoke.ContourArea(approxContour, false);
                        if (contourArea > contourAreaMin && contourArea < contourAreaMax)
                        {
                            if (approxContour.Size == 4)
                            {
                                bool isRectangle = true;
                                System.Drawing.Point[] pts = approxContour.ToArray();
                                LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                                for (int j = 0; j < edges.Length; j++)
                                {
                                    double angle = Math.Abs(
                                       edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                    if (angle < angleMin || angle > angleMax)
                                    {
                                        isRectangle = false;
                                        break;
                                    }
                                }
                                foreach (RotatedRect r in rectangles)
                                {
                                    int x1 = ((approxContour[0].X + approxContour[1].X + approxContour[2].X + approxContour[3].X) / 4);
                                    int y1 = ((approxContour[0].Y + approxContour[1].Y + approxContour[2].Y + approxContour[3].Y) / 4);

                                    for (int x = -5; x <= 5; x++)
                                    {
                                        for (int y = -5; y <= 5; y++)
                                        {
                                            if (x1 == Convert.ToInt32(r.Center.X) + x && y1 == Convert.ToInt32(r.Center.Y) + y)
                                            {
                                                alreadyExists = true;
                                            }
                                        }
                                    }
                                }
                                if (isRectangle && alreadyExists == false)
                                {
                                    rectangles.Add(CvInvoke.MinAreaRect(approxContour));
                                }
                                alreadyExists = false;
                            }
                        }
                    }
                }
            }

            return rectangles;
        }

        public Image<Bgr, byte> GetDrawingRectangles(List<RotatedRect> rectangles, bool ShowCenterPointsOfRects = true)
        {
            Image<Bgr, byte> outputImage = image.Copy();
            SquareTo64Squares(rectangles); //test
            //CENTERPOINTS
            if (ShowCenterPointsOfRects)
            {
                foreach (RotatedRect box in rectangles)
                {
                    RotatedRect r = new RotatedRect();
                    r.Center = box.Center;
                    r.Size = new SizeF(3.0f, 3.0f);
                    outputImage.Draw(r, new Bgr(Color.DarkOrange), 2);
                }
            }

            //EDGES
            foreach (RotatedRect box in rectangles)
            {
                outputImage.Draw(box, new Bgr(Color.DarkOrange), 2);
            }

            return outputImage;
        }

        public void SquareTo64Squares(List<RotatedRect> rectangles)
        {
            int size = rectangles.Count;
            List<RotatedRect>[] littleSquares = new List<RotatedRect>[size];
            for (int i = 0; i < size; i++)
                littleSquares[i] = new List<RotatedRect>();

            int counter = 0;
            foreach (RotatedRect box in rectangles)
            {
                //definition of board size
                var vertices = box.GetVertices();

                var compareForRight1 = Math.Max(vertices[0].X, vertices[1].X);
                var compareForRight2 = Math.Max(vertices[2].X, vertices[3].X);
                var extremeRight = Math.Max(compareForRight1, compareForRight2);
                var compareForLeft1 = Math.Min(vertices[0].X, vertices[1].X);
                var compareForLeft2 = Math.Min(vertices[2].X, vertices[3].X);
                var extremeLeft = Math.Min(compareForLeft1, compareForLeft2);

                var width_board = Math.Abs(extremeRight - extremeLeft);

                var compareForBottom1 = Math.Max(vertices[0].Y, vertices[1].Y);
                var compareForBottom2 = Math.Max(vertices[2].Y, vertices[3].Y);
                var extremeDown = Math.Max(compareForBottom1, compareForBottom2);
                var compareForTop1 = Math.Min(vertices[0].Y, vertices[1].Y);
                var compareForTop2 = Math.Min(vertices[2].Y, vertices[3].Y);
                var extremeUp = Math.Min(compareForTop1, compareForTop2);

                var height_board = Math.Abs(extremeDown - extremeUp);
                //

                //size one square
                var width_square = width_board / 8;
                var height_square = height_board / 8;
                //

                //finding index of left-top vertice
                var list = new List<float> { vertices[0].X, vertices[1].X, vertices[2].X, vertices[3].X };
                var list_copy = new List<float> { vertices[0].X, vertices[1].X, vertices[2].X, vertices[3].X };
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
                //

                float offset_X = 0; float offset_Y = 0;
                for (int i = 0; i < 64; i++)
                {
                    System.Drawing.PointF[] pts = new System.Drawing.PointF[4];
                    pts[0] = new System.Drawing.PointF(vertices[index].X + offset_X, vertices[index].Y + offset_Y);
                    pts[1] = new System.Drawing.PointF(vertices[index].X + offset_X + width_square, vertices[index].Y + offset_Y);
                    pts[2] = new System.Drawing.PointF(vertices[index].X + offset_X, vertices[index].Y + offset_Y + height_square);
                    pts[3] = new System.Drawing.PointF(vertices[index].X + offset_X + width_square, vertices[index].Y + offset_Y + height_square);
                    littleSquares[counter].Add(CvInvoke.MinAreaRect(pts));
                    offset_X += width_square;
                    if ((i+1) % 8 == 0 && i != 0)
                    {
                        offset_X = 0;
                        offset_Y += height_square;
                    }
                }
                counter++;
            }

            Image<Bgr, byte> outputImage = image.Copy();
            if (littleSquares.Count() > 0)
            {
                foreach (RotatedRect box in littleSquares[0])
                {
                    outputImage.Draw(box, new Bgr(Color.Green), 2);
                }
                CvInvoke.Imshow("One square to 64 squares", outputImage);
            }
        }

        public UMat GetCanny(double cannyThreshold = 140.0, double cannyThresholdLinking = 120.0)
        {
            UMat cannyEdges = new UMat();
            CvInvoke.Canny(image, cannyEdges, cannyThreshold, cannyThresholdLinking);

            return cannyEdges;
        }
    }
}
