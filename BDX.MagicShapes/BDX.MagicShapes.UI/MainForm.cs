using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace BDX.MagicShapes.UI
{
    public partial class MainForm : Form
    {
        private Point currentPoint, mouseDownPoint, mouseUpPoint;
        private bool mouseDown, zoomed;
        private Graphics graphicsCanvas;
        private Pen borderPen;
        private SolidBrush brush;
        private Rectangle rectangle;
        private LinkedList<Rectangle> rectangles;

        public MainForm()
        {
            InitializeComponent();
            rectangles = new LinkedList<Rectangle>();
            graphicsCanvas = this.canvasPanel.CreateGraphics();
            graphicsCanvas.InterpolationMode = InterpolationMode.HighQualityBilinear;
            borderPen = new Pen(Color.Olive, 2);
            brush = new SolidBrush(Color.FromArgb(128, 0, 0, 255));
        }

        private void buttonZoom_Click(object sender, EventArgs e)
        {
            zoomed = !zoomed;
            if (zoomed)
            {
                buttonZoom.Text = "Zoom Out";
            }
            else
            {
                buttonZoom.Text = "Zoom In";
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            currentPoint = canvasPanel.PointToClient(Cursor.Position);
            if (mouseDown)
            {
                DrawRectangle();
            }
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDownPoint = canvasPanel.PointToClient(Cursor.Position);
            mouseDown = true;
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUpPoint = canvasPanel.PointToClient(Cursor.Position);
            AddRectangle();
            mouseDown = false;
        }

        private void DrawRectangle()
        {
            graphicsCanvas.Clear(Color.White);
            //Draw previous rectangles
            LoadRectangles();
            Point locationPoint = new Point(Math.Min(mouseDownPoint.X, currentPoint.X), Math.Min(mouseDownPoint.Y, currentPoint.Y));
            int width = Math.Abs(mouseDownPoint.X - currentPoint.X);
            int height = Math.Abs(mouseDownPoint.Y - currentPoint.Y);
            rectangle = new Rectangle(locationPoint.X, locationPoint.Y, width, height);
            graphicsCanvas.FillRectangle(brush, rectangle);
            graphicsCanvas.DrawRectangle(borderPen, rectangle);
        }

        private void LoadRectangles()
        {
            foreach (Rectangle rectangle in rectangles)
            {
                graphicsCanvas.FillRectangle(brush, rectangle);
                graphicsCanvas.DrawRectangle(borderPen, rectangle);
            }
        }

        private void AddRectangle()
        {
            Boolean acceptedRectangle = true;
            foreach (Rectangle listRectangle in rectangles)
            {
                if (Overlapses(rectangle, listRectangle) || Overlapses(listRectangle, rectangle))
                {
                    MessageBox.Show("Rectangle overlapses with other...");
                    graphicsCanvas.Clear(Color.White);
                    LoadRectangles();
                    acceptedRectangle = false;
                    return;
                }
                else
                {
                    //If rectangle has common borders with others
                    if (InAcceptableRange(rectangle.X, listRectangle.X) && InAcceptableRange(rectangle.Y + rectangle.Height, listRectangle.Y))
                    {
                        MessageBox.Show("On top of another");
                    }
                    else if (InAcceptableRange(rectangle.X, listRectangle.X) && InAcceptableRange(rectangle.Y, listRectangle.Y + listRectangle.Height))
                    {
                        MessageBox.Show("On bottom of another");
                    }
                    else if (InAcceptableRange(rectangle.X, listRectangle.X + listRectangle.Width) && InAcceptableRange(rectangle.Y, listRectangle.Y))
                    {
                        MessageBox.Show("On right of another");
                    }
                    else if (InAcceptableRange(rectangle.X + rectangle.Width, listRectangle.X) && InAcceptableRange(rectangle.Y, listRectangle.Y))
                    {
                        MessageBox.Show("On left of another");
                    }
                }
            }
            if (acceptedRectangle)
                rectangles.AddLast(rectangle);
        }

        private bool InAcceptableRange(int numberToCheck, int compareTo)
        {
            int acceptedVariation = 3;
            int bottom = compareTo - acceptedVariation;
            int top = compareTo + acceptedVariation;
            return (numberToCheck >= bottom && numberToCheck <= top);
        }

        private bool Overlapses(Rectangle rectangleA, Rectangle rectangleB)
        {
            if ((rectangleA.X > rectangleB.X && rectangleA.X < rectangleB.X + rectangleB.Width && rectangleA.Y > rectangleB.Y && rectangleA.Y < rectangleB.Y + rectangleB.Height)
                || (rectangleA.X+rectangleA.Width > rectangleB.X && rectangleA.X+rectangleA.Width < rectangleB.X + rectangleB.Width && rectangleA.Y > rectangleB.Y && rectangleA.Y < rectangleB.Y + rectangleB.Height))
            {
                return true;
            }
            return false;
        }
    }
}
