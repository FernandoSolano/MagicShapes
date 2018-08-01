using BDX.MagicShapes.Core.Data;
using BDX.MagicShapes.Core.Model;
using System;

namespace BDX.MagicShapes.Core.Business
{
    public class RectangleBusiness
    {
        private RectangleData rectangleData;

        public RectangleBusiness()
        {
            rectangleData = new RectangleData();
        }

        public Boolean Store(AppState appState, String path)
        {
            return rectangleData.Store(appState, path);
        }

        public AppState Retrieve(String path)
        {
            return rectangleData.Retrieve(path);
        }
    }
}
