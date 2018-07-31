using BDX.MagicShapes.Core.Data;
using BDX.MagicShapes.Core.Model;
using System;
using System.Collections.Generic;

namespace BDX.MagicShapes.Core.Business
{
    public class PolygonBusiness
    {
        private PolygonData polygonData;

        public PolygonBusiness()
        {
            polygonData = new PolygonData();
        }

        public Boolean Store(LinkedList<Polygon> polygons, String path)
        {
            return polygonData.Store(polygons, path);
        }

        public LinkedList<Polygon> Retrieve(String path)
        {
            return polygonData.Retrieve(path);
        }
    }
}
