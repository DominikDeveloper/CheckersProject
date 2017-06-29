using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersApplication
{
    public class CircParam
    {
        public double cannyThreshold { get; }
        public double circleAccumulatorThreshold { get; }
        public double dp { get; }
        public double minDist { get; }
        public int minRadius { get; }
        public int maxRadius { get; }

        public CircParam()
        {
            cannyThreshold = 130.0;
            circleAccumulatorThreshold = 40.0;
            dp = 2.0;
            minDist = 30.0;
            minRadius = 5;
            maxRadius = 30;
        }
    }

}
