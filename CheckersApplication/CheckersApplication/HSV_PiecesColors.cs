using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersApplication
{
    public enum ColorValArray
    {
        Minimum = 0,
        Maximum = 1
    }

    public static class HSV_PiecesColors
    {
        public static Emgu.CV.Structure.MCvScalar[] Player1_MCvS_Data = new Emgu.CV.Structure.MCvScalar[2]; //0 - array of min color values, 1 - max
        public static Emgu.CV.Structure.MCvScalar[] Player2_MCvS_Data = new Emgu.CV.Structure.MCvScalar[2]; //0 - array of min color values, 1 - max
    }
}
