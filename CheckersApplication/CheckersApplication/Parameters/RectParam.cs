using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersApplication
{
    public class RectParam
    {
        public int contourAreaMin { get; }
        public int contourAreaMax { get; }
        public int angleMin { get; }
        public int angleMax { get; }

        public double cannyThresholdLinking { get; }
        public double cannyThreshold { get; }

        public RectParam()
        {
            contourAreaMin = 10000;
            contourAreaMax = 200000;
            angleMin = 85;
            angleMax = 95;

            cannyThresholdLinking = 120.0;
            cannyThreshold = 140.0;
        }
    }
}
