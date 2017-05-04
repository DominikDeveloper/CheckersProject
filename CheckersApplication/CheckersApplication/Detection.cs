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
        Random rnd = new Random();
        int ContourAreaMin;
        int ContourAreaAddtoMin;
        int AngleMin;
        int AngleMax;
        int calibrationRounds = 0;
        int ChessboardIndex1 = 7;
        int ChessboardIndex2 = 7;
        ChessField[,] ChessboardArray = new ChessField[8, 8];

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
                if (!result)
                    MessageBox.Show("Nie wykryto rogów szachownicy");
                return editableImage;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return null;
        }

        //Convert the image to grayscale and filter out the noise
        public UMat ConvertClearImage(Image<Bgr, Byte> img)
        {
            UMat uimage = new UMat();
            CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);
            UMat pyrDown = new UMat();            //use image pyr to remove noise
            CvInvoke.PyrDown(uimage, pyrDown);
            CvInvoke.PyrUp(pyrDown, uimage);
            return uimage;
        }

        public ChessField[,] RepresentCircles(CircleF[] circles)
        {
            ChessField[,] chessFields = new ChessField[8,8];
            foreach (CircleF circle in circles)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        chessFields[i, j] = new ChessField();
                        int x1 = Convert.ToInt32(circle.Center.X);
                        int y1 = Convert.ToInt32(circle.Center.Y);
                        for (int x = -7; x <= 7; x++)
                        {
                            for (int y = -7; y <= 7; y++)
                            {
                                if (x1 == chessFields[i, j].x + x && y1 == chessFields[i, j].y + y)
                                {
                                    chessFields[i, j].Value = 2;
                                }
                            }
                        }
                    }
                }
            }
            return chessFields;
        }

        public CircleF[] GetCircles(ref Image<Bgr, Byte> img, int thickness = 3)
        {
            UMat uimage = ConvertClearImage(img);
            double cannyThreshold = 140.0;
            double circleAccumulatorThreshold = 40.0;
            double dp = 2.0;
            double minDist = 30.0;
            int minRadius = 5;
            int maxRadius = 30;
            CircleF[] circles = CvInvoke.HoughCircles(
                uimage, HoughType.Gradient, dp, minDist, cannyThreshold, circleAccumulatorThreshold, minRadius, maxRadius);

            foreach (CircleF circle in circles)
                img.Draw(circle, new Bgr(Color.Blue), thickness);

            return circles;
        }

        public List<RotatedRect> GetRectangles(ref Image<Bgr, Byte> img)
        {
            double cannyThresholdLinking = 120.0;
            double cannyThreshold = 140.0;

            Image<Bgr, Byte> img3 = img.CopyBlank();

            UMat cannyEdges = new UMat();
            CvInvoke.Canny(img, cannyEdges, cannyThreshold, cannyThresholdLinking);

            LineSegment2D[] lines = CvInvoke.HoughLinesP(
               cannyEdges,
               1, //Distance resolution in pixel-related units
               Math.PI / 45.0, //Angle resolution measured in radians.
               20, //threshold
               30, //min Line width
               10); //gap between lines

            List<RotatedRect> boxList = new List<RotatedRect>(); //a box is a rotated rectangle

            while (boxList.Count() != 64 && calibrationRounds < 3000)
            {
                calibrationRounds++;
                ContourAreaMin = rnd.Next(1, 1000);
                ContourAreaAddtoMin = rnd.Next(1, 3000);
                AngleMin = 85;
                AngleMax = 95;
                
                using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                {
                    CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                    int count = contours.Size;
                    for (int i = 0; i < count; i++)
                    {
                        using (VectorOfPoint contour = contours[i])
                        using (VectorOfPoint approxContour = new VectorOfPoint())
                        {
                            CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
                            if (CvInvoke.ContourArea(approxContour, false) > ContourAreaMin && CvInvoke.ContourArea(approxContour, false) < ContourAreaMin + ContourAreaAddtoMin)
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
                                        if (angle < AngleMin || angle > AngleMax)
                                        {
                                            isRectangle = false;
                                            break;
                                        }
                                    }
                                    foreach (RotatedRect r in boxList)
                                    {
                                        int x1 = ((approxContour[0].X + approxContour[1].X + approxContour[2].X + approxContour[3].X) / 4);
                                        int y1 = ((approxContour[0].Y + approxContour[1].Y + approxContour[2].Y + approxContour[3].Y) / 4);

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
                                    if (isRectangle && RectangleExists == 0 && ChessboardIndex1 >= 0)
                                    {
                                        boxList.Add(CvInvoke.MinAreaRect(approxContour));
                                    }
                                    RectangleExists = 0;
                                }
                            }
                        }
                    }
                }

            }



            foreach (RotatedRect box in boxList)
            {
                img.Draw(box, new Bgr(Color.DarkOrange), 1);
            }

            return boxList;
        }
    }
}