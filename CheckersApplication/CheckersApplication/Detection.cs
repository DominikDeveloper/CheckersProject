using System;
using System.Drawing;
using System.Windows;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Collections.Generic;

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

            CircParam circParams = new CircParam();

            CircleF[] circles = CvInvoke.HoughCircles(
                uimage, HoughType.Gradient, 
                circParams.dp, circParams.minDist, circParams.cannyThreshold, circParams.circleAccumulatorThreshold,
                circParams.minRadius, circParams.maxRadius);

            return circles;
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

        public static void DetectPlayersColors(CircleF []circles, ref System.Windows.Media.Color player1, ref System.Windows.Media.Color player2, Image<Bgr, byte> image)
        {
            int lastChange=2;
            List<Color> currentColors = new List<Color>();
            System.Windows.Media.Color color = new System.Windows.Media.Color();
            if (circles != null)
            {
                foreach (CircleF circle in circles)
                {
                    //resultImage.Draw(circle, new Bgr(Color.Blue), 3);
                    try
                    {
                        currentColors.Clear();
                        for (int i = 0; i < circle.Radius - 2; i++)
                        {
                            currentColors.Add(image.Bitmap.GetPixel((int)circle.Center.X + i, (int)circle.Center.Y));
                            currentColors.Add(image.Bitmap.GetPixel((int)circle.Center.X - i, (int)circle.Center.Y));
                            currentColors.Add(image.Bitmap.GetPixel((int)circle.Center.X, (int)circle.Center.Y + i));
                            currentColors.Add(image.Bitmap.GetPixel((int)circle.Center.X, (int)circle.Center.Y - i));
                            currentColors.Add(image.Bitmap.GetPixel((int)circle.Center.X + i, (int)circle.Center.Y + i));
                            currentColors.Add(image.Bitmap.GetPixel((int)circle.Center.X - i, (int)circle.Center.Y - i));
                            currentColors.Add(image.Bitmap.GetPixel((int)circle.Center.X + i, (int)circle.Center.Y - i));
                            currentColors.Add(image.Bitmap.GetPixel((int)circle.Center.X - i, (int)circle.Center.Y + i));
                        }

                        int r = 0, g = 0, b = 0, counter = 0;
                        foreach (Color clr in currentColors)
                        {
                            counter++;
                            r += (int)clr.R;
                            g += (int)clr.G;
                            b += (int)clr.B;
                        }
                        r = r / counter;
                        g = g / counter;
                        b = b / counter;
                        CircleF c = new CircleF(circle.Center, 5);
                        //resultImage.Draw(c, new Bgr(Color.Yellow), 3);
                        color = System.Windows.Media.Color.FromRgb((byte)r, (byte)g, (byte)b);


                        if (player1.R > color.R - 50 &&
                            player1.R < color.R + 50 &&
                            player1.G > color.G - 50 &&
                            player1.G < color.G + 50 &&
                            player1.B > color.B - 50 &&
                            player1.B < color.B + 50 ||
                            player2.R > color.R - 50 &&
                            player2.R < color.R + 50 &&
                            player2.G > color.G - 50 &&
                            player2.G < color.G + 50 &&
                            player2.B > color.B - 50 &&
                            player2.B < color.B + 50)
                            continue;
                        else if (MainWindow.player1Detected == false)
                        {
                            player1 = color;
                            lastChange = 1;
                            MainWindow.player1Detected = true;
                            continue;
                        }
                        else if (MainWindow.player2Detected == false)
                        {
                            player2 = color;
                            lastChange = 2;
                            MainWindow.player2Detected = true;
                        }
                        else
                        {
                            if (lastChange == 1)
                            {
                                player2 = color;
                                lastChange = 2;
                            }

                            else{
                                player1 = color;
                                lastChange = 1;
                            }

                            MainWindow.player1Detected = true;
                        }
                    }
                    catch (Exception e)
                    {

                    }

                }
            }
        }


        public static CircleF[] FilterSomeColors(Image<Bgr, Byte> image, ref Mat effectMat, double[] minColorsValues, double[] maxColorsValues)
        {
            // Load input image
            Mat bgr_image = image.Mat;

            Mat orig_image = bgr_image.Clone();

            CvInvoke.MedianBlur(bgr_image, bgr_image, 3);

            // Threshold the BGR image, keep only the selected color pixels
            Mat color_hue_image = new Mat();
            
            CvInvoke.InRange(
                bgr_image, 
                new ScalarArray(new MCvScalar(Convert.ToInt32(minColorsValues[0]), Convert.ToInt32(minColorsValues[1]), Convert.ToInt32(minColorsValues[2]))),
                new ScalarArray(new MCvScalar(Convert.ToInt32(maxColorsValues[0]), Convert.ToInt32(maxColorsValues[1]), Convert.ToInt32(maxColorsValues[2]))),
                color_hue_image);

            CvInvoke.GaussianBlur(color_hue_image, color_hue_image, new System.Drawing.Size(9, 9), 2, 2);

            effectMat = color_hue_image;

            // Use the Hough transform to detect circles in the combined threshold image
            //var circlesArray = CvInvoke.HoughCircles(color_hue_image, HoughType.Gradient, 1, color_hue_image.Rows / 8, 100, 20, 0, 0);

            CircParam circParams = new CircParam();

            CircleF[] circlesArray = CvInvoke.HoughCircles(
                color_hue_image, HoughType.Gradient, 
                circParams.dp, circParams.minDist, circParams.cannyThreshold, circParams.circleAccumulatorThreshold, 
                circParams.minRadius, circParams.maxRadius);

            return circlesArray;
        }

        public static System.Drawing.Point[] GetRectanglePoints(Image<Bgr, Byte> img)
        {

            RectParam rectParams = new RectParam();

            UMat cannyEdges = new UMat();

            CvInvoke.Canny(img, cannyEdges, rectParams.cannyThreshold, rectParams.cannyThresholdLinking);

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
                            if (contourArea > rectParams.contourAreaMin && contourArea < rectParams.contourAreaMax)
                            {
                                if (approxContour.Size == 4)
                                {
                                    points  = approxContour.ToArray();
                                    LineSegment2D[] edges = PointCollection.PolyLine(points, true);
                                    if(Detection.CheckAngles(edges, rectParams.angleMin, rectParams.angleMax))
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