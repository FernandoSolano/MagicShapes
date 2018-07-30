using BDX.MagicShapes.Core.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDX.MagicShapes.Core.Business
{
    public class RectangleBusiness
    {
        private RectangleData rectangleData;

        public RectangleBusiness()
        {
            rectangleData = new RectangleData();
        }

        public Boolean Store(LinkedList<Rectangle> rectangles)
        {
            return rectangleData.Store(rectangles);
        }

        public LinkedList<Rectangle> Retrieve()
        {
            return rectangleData.Retrieve();
        }
    }
}
