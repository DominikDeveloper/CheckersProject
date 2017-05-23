using System;
using System.Drawing;
using System.Windows;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace CheckersApplication
{
    abstract class Detection
    {
        //Convert the image to grayscale and filter out the noise
        public static UMat ConvertClearImage(Image<Bgr, Byte> img)
        {
            UMat uimage = new UMat();
            CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);
            UMat pyrDown = new UMat();            //use image pyr to remove noise
            CvInvoke.PyrDown(uimage, pyrDown);
            CvInvoke.PyrUp(pyrDown, uimage);
            return uimage;
        }

        public static CircleF[] GetCircles(Image<Bgr, Byte> img)
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

            return circles;
        }

        public static CircleF[] GetColorCircles(Image<Bgr, Byte> image, MCvScalar[] mcvs)
        {
            // Load input image
            Mat bgr_image = image.Mat;

            Mat orig_image = bgr_image.Clone();

            CvInvoke.MedianBlur(bgr_image, bgr_image, 3);

            // Convert input image to HSV
            Mat hsv_image = new Mat();
            CvInvoke.CvtColor(bgr_image, hsv_image, ColorConversion.Bgr2Hsv);

            // Threshold the HSV image, keep only the selected color pixels
            Mat color_hue_image = new Mat();

            CvInvoke.InRange(hsv_image, new ScalarArray(mcvs[0]), new ScalarArray(mcvs[1]), color_hue_image);

            // GaussianBlur
            CvInvoke.GaussianBlur(color_hue_image, color_hue_image, new System.Drawing.Size(9, 9), 2, 2);

            // Use the Hough transform to detect circles in the combined threshold image
            var circ = CvInvoke.HoughCircles(color_hue_image, HoughType.Gradient, 1, color_hue_image.Rows / 8, 100, 20, 0, 0); 

            // Return detected circles
            if (circ.Length == 0)
                return (new CircleF[0]);

            return circ;
        }

        public static Mat DrawDetectedColorCircle(Bitmap bmp, String[] minHSV, String[] maxHSV, ref MCvScalar[] mcvs)
        {
            var fromCam = new Image<Bgr, byte>(bmp);

            Mat bgr_image = fromCam.Mat;

            Mat orig_image = bgr_image.Clone();

            CvInvoke.MedianBlur(bgr_image, bgr_image, 3);

            // Convert input image to HSV
            Mat hsv_image = new Mat();
            CvInvoke.CvtColor(bgr_image, hsv_image, ColorConversion.Bgr2Hsv);

            // Threshold the HSV image, keep only the selected color pixels
            Mat color_hue_image = new Mat();

            // Color values
            try
            {
                mcvs = new MCvScalar[2] {
                        new MCvScalar(Convert.ToInt32(minHSV[0]), Convert.ToInt32(minHSV[1]), Convert.ToInt32(minHSV[2])),
                        new MCvScalar(Convert.ToInt32(maxHSV[0]), Convert.ToInt32(maxHSV[1]), Convert.ToInt32(maxHSV[2]))
                    };

            }
            catch (Exception)
            {
                //do nothing
            }
            CvInvoke.InRange(hsv_image, new ScalarArray(mcvs[0]), new ScalarArray(mcvs[1]), color_hue_image);
            CvInvoke.GaussianBlur(color_hue_image, color_hue_image, new System.Drawing.Size(9, 9), 2, 2);

            // Use the Hough transform to detect circles in the combined threshold image
            var circles = new VectorOfPoint3D32F();
            CvInvoke.HoughCircles(color_hue_image, circles, HoughType.Gradient, 1, color_hue_image.Rows / 8, 100, 20, 0, 0);

            return color_hue_image;
        }

        private static bool CheckAngles(LineSegment2D[] edges, int angleMin, int angleMax)
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

        public static System.Drawing.Point[] GetRectanglePoints(Image<Bgr, Byte> img)
        {
            double cannyThresholdLinking = 120.0;
            double cannyThreshold = 140.0;

            UMat cannyEdges = new UMat();

            CvInvoke.Canny(img, cannyEdges, cannyThreshold, cannyThresholdLinking);

                int contourAreaMin = 10000;
                int contourAreaMax = 200000;
                int angleMin = 85;
                int angleMax = 95;
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
                                    if(Detection.CheckAngles(edges, angleMin, angleMax))
                                    {
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