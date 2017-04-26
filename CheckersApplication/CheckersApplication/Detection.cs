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
        int g = 0;
        int x1;
        int y1;
        Random rnd = new Random();
        int rnd1;
        int rnd2;
        int rnd3;
        int rnd4;
        int rnd5;


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

            /*while (boxList.Count() != 64)
            {*/
                rnd1 = rnd.Next(1, 101); //?
                rnd2 = 500;//rnd.Next(1, 1000);
                rnd3 = 3500; //rnd.Next(1, 3000);
                rnd4 = 85; //rnd.Next(70, 90);
                rnd5 = 95; //rnd.Next(90, 110);
                
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
                            if (CvInvoke.ContourArea(approxContour, false) > rnd2 && CvInvoke.ContourArea(approxContour, false) < rnd2 + rnd3) //only consider contours with area greater than 250
                            {
                                if (approxContour.Size == 3) //The contour has 3 vertices, it is a triangle
                                {
                                    System.Drawing.Point[] pts = approxContour.ToArray();
                                    triangleList.Add(new Triangle2DF(
                                       pts[0],
                                       pts[1],
                                       pts[2]
                                       ));
                                }
                                else if (approxContour.Size == 4) //The contour has 4 vertices.
                                {
                                    #region determine if all the angles in the contour are within [80, 100] degree
                                    bool isRectangle = true;
                                    System.Drawing.Point[] pts = approxContour.ToArray();
                                    LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                                    for (int j = 0; j < edges.Length; j++)
                                    {
                                        double angle = Math.Abs(
                                           edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                        if (angle < rnd4 || angle > rnd5)
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
                                                    g = 1;
                                                }
                                            }
                                        }
                                    }
                                    if (isRectangle && g == 0) boxList.Add(CvInvoke.MinAreaRect(approxContour));
                                    g = 0;
                                }
                            }
                        }
                    }
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