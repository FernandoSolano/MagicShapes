using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

namespace BDX.MagicShapes.Core.Model
{
    public class AppState
    {
        public List<Rectangle> Rectangles { get; set; }
        public List<Rectangle> AuxRectangles { get; set; }

        public AppState()
        {
            Rectangles = new List<Rectangle>();
            AuxRectangles = new List<Rectangle>();
        }

        public AppState(List<Rectangle> Rectangles, List<Rectangle> AuxRectangles)
        {
            this.Rectangles = Rectangles;
            this.AuxRectangles = AuxRectangles;
        }
    }
}
