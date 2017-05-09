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

        public List<RotatedRect> GetRectangles()
        {
            double cannyThresholdLinking = 120.0;
            double cannyThreshold = 140.0;
            bool alreadyExists = false;

            Image<Bgr, Byte> img = image.Copy();

            Image<Bgr, Byte> img3 = img.CopyBlank();

            UMat cannyEdges = new UMat();
            CvInvoke.Canny(img, cannyEdges, cannyThreshold, cannyThresholdLinking);
            CvInvoke.Imshow("Canny edges", cannyEdges);

            List<RotatedRect> rectangles = new List<RotatedRect>(); //a box is a rotated rectangle

            int contourAreaMin = 300;
            int contourAreaMax = 20000;
            int angleMin = 75;
            int angleMax = 105;

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

        public Image<Bgr, byte> GetDrawingRectangles(List<RotatedRect> rectangles)
        {
            Image<Bgr, byte> outputImage = image.Copy();

            //CENTERPOINTS
            foreach (RotatedRect box in rectangles)
            {
                RotatedRect r = new RotatedRect();
                r.Center = box.Center;
                r.Size = new SizeF(3.0f, 3.0f);
                outputImage.Draw(r, new Bgr(Color.DarkOrange), 2);
            }

            //EDGES
            foreach (RotatedRect box in rectangles)
            {
                outputImage.Draw(box, new Bgr(Color.DarkOrange), 2);
            }

            return outputImage;
        }
    }
}
