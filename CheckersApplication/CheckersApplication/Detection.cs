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

        bool CheckAngles(LineSegment2D[] edges, int angleMin, int angleMax)
        {
            for (int j = 0; j < edges.Length; j++)
            {
                double angle = Math.Abs(
                   edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                if (angle < angleMin || angle > angleMax)
                {
                    return false;
                }
            }
            return true;
        }

        public System.Drawing.Point[] GetRectanglePoints(ref Image<Bgr, Byte> img)
        {
            double cannyThresholdLinking = 120.0;
            double cannyThreshold = 140.0;

            Image<Bgr, Byte> img3 = img.CopyBlank();
            UMat cannyEdges = new UMat();

            CvInvoke.Canny(img, cannyEdges, cannyThreshold, cannyThresholdLinking);
            CvInvoke.Imshow("Canny edges", cannyEdges);

                int contourAreaMin = 10000;
                int contourAreaMax = 200000;
                int angleMin = 75;
                int angleMax = 105;
                System.Drawing.Point[] points = null;
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
                                points  = approxContour.ToArray();
                                LineSegment2D[] edges = PointCollection.PolyLine(points, true);
                                    if(CheckAngles(edges, angleMin, angleMax))
                                    {
                                        img.Draw(points, new Bgr(Color.DarkOrange), 2);
                                        return points;
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                }
                            }
                        }
                    }
                }
            return null;
        }

    }
}