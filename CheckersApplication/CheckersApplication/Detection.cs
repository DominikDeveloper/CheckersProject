using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Interop;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;


namespace CheckersApplication
{
    class Detection
    {
        int RectangleExists = 0;
        int x1;
        int y1;
        Random rnd = new Random();
        //int rnd1;
        int ContourAreaMin;
        int ContourAreaAddtoMin;
        int AngleMin;
        int AngleMax;
        //int MaxRectangles = 0;
        int calibrationRounds = 0;
        int ChessboardIndex1 = 7;
        int ChessboardIndex2 = 7;
        ChessField[,] ChessboardArray = new ChessField[8, 8];
        ChessField ChessField = new ChessField();



        public IInputOutputArray GetInternalCorners(IImage inputImage, UInt16 width, UInt16 height)
        {
            IInputOutputArray editableImage = inputImage;
            System.Drawing.Size patternSize = new System.Drawing.Size(width - 1, height - 1);
            var cornerPoints = new VectorOfPointF();
            bool result = CvInvoke.FindChessboardCorners(
                editableImage, patternSize, cornerPoints, Emgu.CV.CvEnum.CalibCbType.AdaptiveThresh | Emgu.CV.CvEnum.CalibCbType.FilterQuads);
            try
            {
                CvInvoke.DrawChessboardCorners(editableImage, patternSize, cornerPoints, result);
                return editableImage;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            return null;
        }

        public IImage GetCircles(Image<Bgr, Byte> img, int thickness = 3)
        {
            StringBuilder msgBuilder = new StringBuilder("Performance: ");

            //Convert the image to grayscale and filter out the noise
            UMat uimage = new UMat();
            CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

            //use image pyr to remove noise
            UMat pyrDown = new UMat();
            CvInvoke.PyrDown(uimage, pyrDown);
            CvInvoke.PyrUp(pyrDown, uimage);

            //Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();

            #region circle detection
            Stopwatch watch = Stopwatch.StartNew();
            double cannyThreshold = 140.0;
            double circleAccumulatorThreshold = 40.0;
            double dp = 2.0;
            double minDist = 30.0;
            int minRadius = 5;
            int maxRadius = 30;
            CircleF[] circles = CvInvoke.HoughCircles(
                uimage, HoughType.Gradient, dp, minDist, cannyThreshold, circleAccumulatorThreshold, minRadius, maxRadius);


            watch.Stop();
            msgBuilder.Append(String.Format("Hough circles - {0} ms; Number of circles: {1}", watch.ElapsedMilliseconds, circles.Length));
            Console.WriteLine(msgBuilder);
            #endregion

            foreach (CircleF circle in circles)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        x1 = Convert.ToInt32(circle.Center.X);
                        //r.Center.X
                        y1 = Convert.ToInt32(circle.Center.Y);

                        for (int x = -5; x <= 5; x++)
                        {
                            for (int y = -5; y <= 5; y++)
                            {
                                if (x1 == ChessboardArray[i, j].x + x && y1 == ChessboardArray[i, j].y + y)
                                {
                                    ChessboardArray[i, j].Value = 2;
                                }
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Console.WriteLine(ChessboardArray[i, j].Value.ToString());
                }
            }

            #region draw circles
            foreach (CircleF circle in circles)
                img.Draw(circle, new Bgr(Color.Blue), thickness);

            return img;
            #endregion
        }

        public IImage GetTrianglesRectangles(Image<Bgr, Byte> img2, bool BlackBox = true)
        {
            StringBuilder msgBuilder = new StringBuilder("Performance: ");

            double cannyThresholdLinking = 120.0;
            Stopwatch watch = Stopwatch.StartNew();
            double cannyThreshold = 140.0;

            Image<Bgr, Byte> img3 = img2.CopyBlank();

            UMat cannyEdges = new UMat();
            CvInvoke.Canny(img2, cannyEdges, cannyThreshold, cannyThresholdLinking);

            LineSegment2D[] lines = CvInvoke.HoughLinesP(
               cannyEdges,
               1, //Distance resolution in pixel-related units
               Math.PI / 45.0, //Angle resolution measured in radians.
               20, //threshold
               30, //min Line width
               10); //gap between lines



            watch.Restart();
            List<Triangle2DF> triangleList = new List<Triangle2DF>();
            List<RotatedRect> boxList = new List<RotatedRect>(); //a box is a rotated rectangle

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    ChessboardArray[i, j] = ChessField;
                }
            }

            while (boxList.Count() != 64 && calibrationRounds < 3000)
            {
                //boxList.Clear();
                //for(int i1=0;i1<1000 && boxList.Count() != 64; i1++)
                //{
                //    for (int i2 = 0; i2 < 3000 && boxList.Count() != 64; i2++)
                //    {
                //        for (int i3 = 70; i3 < 90 && boxList.Count() != 64; i3++)
                //        {
                //            for (int i4 = 90; i4 < 110 && boxList.Count()!=64; i4++)
                //            {

                calibrationRounds++;
                //rnd1 = rnd.Next(1, 101); //?
                ContourAreaMin = rnd.Next(1, 1000);
                ContourAreaAddtoMin = rnd.Next(1, 3000);
                AngleMin = rnd.Next(70, 90);
                AngleMax = rnd.Next(90, 110);
                
                using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                {
                    CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                    int count = contours.Size;
                    for (int i = 0; i < count; i++)
                    {
                        // if (i % 8 == 0) i += 8;
                        using (VectorOfPoint contour = contours[i])
                        using (VectorOfPoint approxContour = new VectorOfPoint())
                        {
                            CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
                            if (CvInvoke.ContourArea(approxContour, false) > ContourAreaMin && CvInvoke.ContourArea(approxContour, false) < ContourAreaMin + ContourAreaAddtoMin) //only consider contours with area greater than 250
                            {
                                //if (approxContour.Size == 3) //The contour has 3 vertices, it is a triangle
                                //{
                                //    System.Drawing.Point[] pts = approxContour.ToArray();
                                //    triangleList.Add(new Triangle2DF(
                                //       pts[0],
                                //       pts[1],
                                //       pts[2]
                                //       ));
                                //}
                                if (approxContour.Size == 4) //The contour has 4 vertices.
                                {
                                    #region determine if all the angles in the contour are within [80, 100] degree
                                    bool isRectangle = true;
                                    System.Drawing.Point[] pts = approxContour.ToArray();
                                    LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                                    for (int j = 0; j < edges.Length; j++)
                                    {
                                        double angle = Math.Abs(
                                           edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                        if (angle < AngleMin || angle > AngleMax)
                                        {
                                            isRectangle = false;
                                            break;
                                        }
                                    }
                                    #endregion
                                    foreach (RotatedRect r in boxList)
                                    {
                                        x1 = ((approxContour[0].X + approxContour[1].X + approxContour[2].X + approxContour[3].X) / 4);
                                        //r.Center.X
                                        y1 = ((approxContour[0].Y + approxContour[1].Y + approxContour[2].Y + approxContour[3].Y) / 4);

                                        for (int x = -5; x <= 5; x++)
                                        {
                                            for (int y = -5; y <= 5; y++)
                                            {
                                                if (x1 == Convert.ToInt32(r.Center.X) + x && y1 == Convert.ToInt32(r.Center.Y) + y)
                                                {
                                                    RectangleExists = 1;
                                                }
                                            }
                                        }
                                    }
                                    if (isRectangle && RectangleExists == 0)
                                    {
                                        boxList.Add(CvInvoke.MinAreaRect(approxContour));
                                        ChessboardArray[ChessboardIndex1, ChessboardIndex2].Value = 1;
                                        ChessboardArray[ChessboardIndex1, ChessboardIndex2].x = x1;
                                        ChessboardArray[ChessboardIndex1, ChessboardIndex2].y = y1;
                                        ChessboardIndex2--;
                                        if(ChessboardIndex2==-1)
                                        {
                                            ChessboardIndex1--;
                                            ChessboardIndex2 = 7;
                                        }
                                    }
                                    RectangleExists = 0;
                                }
                            }
                        }
                    }
                }
                //            }
                //        }
                //    }
                //}

            }

            watch.Stop();
            msgBuilder.Append(String.Format("Triangles & Rectangles - {0} ms; triangles: {1}, rectangles: {2}", watch.ElapsedMilliseconds, triangleList.Count, boxList.Count));
            Console.WriteLine(msgBuilder);

            Image<Bgr, byte> resultImg;
            if (BlackBox)
            {
                resultImg = img3;
            }
            else
                resultImg = img2;

            //foreach (Triangle2DF triangle in triangleList) { resultImg.Draw(triangle, new Bgr(Color.DarkBlue), 2); }

            foreach (RotatedRect box in boxList)
            {
                resultImg.Draw(box, new Bgr(Color.DarkOrange), 1);
            }

            return resultImg;
        }
    }
}