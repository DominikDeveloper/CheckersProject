using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using System.Windows;

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
            this.patternSize = new System.Drawing.Size(width, height);
            this.ifPatternWasFound = false;
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

        public bool IfPatternFound()
        {
            if (!ifGettingCorners)
            {
                GetInternalCorners();
            }

            return this.ifPatternWasFound;
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
    }
}
