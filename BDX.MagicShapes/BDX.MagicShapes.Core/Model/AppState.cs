using System;
using System.Collections.Generic;
using System.Drawing;

namespace BDX.MagicShapes.Core.Model
{
    [Serializable]
    public class AppState
    {
        private LinkedList<Rectangle> Rectangles, AuxRectangles;

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

        public LinkedList<Rectangle> Rectangles1 { get => Rectangles; set => Rectangles = value; }
        public LinkedList<Rectangle> AuxRectangles1 { get => AuxRectangles; set => AuxRectangles = value; }
    }
}
