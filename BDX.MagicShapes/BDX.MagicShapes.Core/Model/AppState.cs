using System;
using System.Collections.Generic;
using System.Drawing;

namespace BDX.MagicShapes.Core.Model
{
    [Serializable]
    public class AppState
    {

        public LinkedList<Rectangle> Rectangles { get; set; }
        public LinkedList<Rectangle> AuxRectangles { get; set; }

        public AppState()
        {
            Rectangles = new LinkedList<Rectangle>();
            AuxRectangles = new LinkedList<Rectangle>();
        }

        public AppState(LinkedList<Rectangle> Rectangles, LinkedList<Rectangle> AuxRectangles)
        {
            this.Rectangles = Rectangles;
            this.AuxRectangles = AuxRectangles;
        }
    }
}
