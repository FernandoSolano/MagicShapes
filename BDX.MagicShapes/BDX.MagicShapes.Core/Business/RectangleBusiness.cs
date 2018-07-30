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

        public Boolean Store(LinkedList<Rectangle> rectangles, String path)
        {
            return rectangleData.Store(rectangles, path);
        }

        public LinkedList<Rectangle> Retrieve(String path)
        {
            return rectangleData.Retrieve(path);
        }
    }
}
