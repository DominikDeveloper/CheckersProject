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
        public void ShowCorners(IInputOutputArray imageObj, UInt16 width, UInt16 height) //terrible delay
        {
            System.Drawing.Size patternSize = new System.Drawing.Size(width - 1, height - 1);
            var cornerPoints = new VectorOfPointF();
            bool result = CvInvoke.FindChessboardCorners(imageObj, patternSize, cornerPoints, Emgu.CV.CvEnum.CalibCbType.AdaptiveThresh | Emgu.CV.CvEnum.CalibCbType.FilterQuads);
            try
            {
                CvInvoke.DrawChessboardCorners(imageObj, patternSize, cornerPoints, result);
                CvInvoke.Imshow("Result of corners browsing", imageObj);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public void ShowCircles()
        {
            StringBuilder msgBuilder = new StringBuilder("Performance: ");

            //Load the image from file and resize it for display
            Image<Bgr, Byte> img =
               new Image<Bgr, byte>("Chessboard.png")
               .Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);
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
            CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, dp, minDist, cannyThreshold, circleAccumulatorThreshold, minRadius, maxRadius);

            watch.Stop();
            msgBuilder.Append(String.Format("Hough circles - {0} ms; Number of circles: {1}", watch.ElapsedMilliseconds,circles.Length));
            Console.WriteLine(msgBuilder);

            #endregion
            #region draw circles
            foreach (CircleF circle in circles)
                img.Draw(circle, new Bgr(Color.Blue), 3);

            CvInvoke.Imshow("Result of circles browsing", img);
            #endregion
        }
    }
}
