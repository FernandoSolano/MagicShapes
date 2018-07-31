using System;
using System.Collections.Generic;
using System.Drawing;

namespace BDX.MagicShapes.Core.Model
{
    public class Polygon
    {
        public Polygon()
        {
            Points = new List<Point>();
        }

        public List<Point> Points { get; set; }

        public void sortPoints()
        {
            Points.Sort(OrderByAxis);
        }

        private int OrderByAxis(Point pointA, Point pointB)
        {
            if (pointA.X <= pointB.X && pointA.Y<=pointB.Y)
            {
                return 1;
            }
            else if (pointA.X <= pointB.X && pointA.Y>pointB.Y)
            {
                return -1;
            }
            return 0;
        }
    }
}
