using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace CheckersApplication
{
    class Detection
    {
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

        public int[,] RepresentCircles(CircleF[] circles, List<RotatedRect> rectangles)
        {
            int[,] matrix = new int[8, 8];
            foreach (CircleF circle in circles)
            {
                int circleX = Convert.ToInt32(circle.Center.X);
                int circleY = Convert.ToInt32(circle.Center.Y);
                foreach(var rectangle in rectangles)
                {
                    int rectangleX = Convert.ToInt32(rectangle.Center.X);
                    int rectangleY = Convert.ToInt32(rectangle.Center.Y);
                    for (int x = -7; x <= 7; x++)
                    {
                        for (int y = -7; y <= 7; y++)
                        {
                            if (circleX == rectangleX + x && circleY == rectangleY + y)
                            {                               
                                matrix[0, 0] = 2;
                            }
                        }
                    }
                }
            }
            return matrix;
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
            bool alreadyExists = false;

            Image<Bgr, Byte> img3 = img.CopyBlank();

            UMat cannyEdges = new UMat();
            CvInvoke.Canny(img, cannyEdges, cannyThreshold, cannyThresholdLinking);
            CvInvoke.Imshow("Canny edges", cannyEdges);
            LineSegment2D[] lines = CvInvoke.HoughLinesP(
               cannyEdges,
               1, //Distance resolution in pixel-related units
               Math.PI / 45.0, //Angle resolution measured in radians.
               20, //threshold
               30, //min Line width
               10); //gap between lines

            List<RotatedRect> boxList = new List<RotatedRect>(); //a box is a rotated rectangle

                int contourAreaMin = 300;
                int contourAreaMax = 20000;
                int angleMin = 75;
                int angleMax = 105;
                
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
                                                    alreadyExists = true;
                                                }
                                            }
                                        }
                                    }
                                    if (isRectangle && alreadyExists == false)
                                    {
                                        boxList.Add(CvInvoke.MinAreaRect(approxContour));
                                    }
                                 alreadyExists = false;
                                }
                            }
                        }
                    }
                }


            foreach (RotatedRect box in boxList)
            {
                RotatedRect r = new RotatedRect();
                r.Center = box.Center;
                r.Size = new SizeF(3.0f,3.0f);
                img.Draw(r, new Bgr(Color.DarkOrange), 2);
            }

            foreach (RotatedRect box in boxList)
            {
                img.Draw(box, new Bgr(Color.DarkOrange), 2);
            }

                return boxList;
        }
    }
}