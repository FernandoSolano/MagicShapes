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
    }
}
