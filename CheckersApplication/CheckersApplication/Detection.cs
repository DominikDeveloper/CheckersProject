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
    }
}