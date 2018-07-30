using BDX.MagicShapes.Core.Business;
using BDX.MagicShapes.UI.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BDX.MagicShapes.UI
{
    public partial class MainForm : Form
    {
        private Point CurrentPoint, MouseDownPoint;
        private bool MouseClickHolded, ValidSize, Zoomed;
        private Graphics GraphicsCanvas;
        private Pen BorderPen;
        private SolidBrush Brush;
        private Rectangle Rectangle;
        private LinkedList<Rectangle> Rectangles;
        private float ZoomValue;

        public MainForm()
        {
            InitializeComponent();
            Rectangles = new LinkedList<Rectangle>();
            GraphicsCanvas = this.canvasPanel.CreateGraphics();
            GraphicsCanvas.InterpolationMode = InterpolationMode.HighQualityBilinear;
            ZoomValue = 1f;
            Zoomed = false;
            BorderPen = new Pen(Color.Olive, 1);
            Brush = new SolidBrush(Color.FromArgb(128, 0, 0, 255));
        }

        private void buttonZoom_Click(object sender, EventArgs e)
        {
            Zoomed = !Zoomed;
            if (Zoomed)
            {
                ZoomValue = 10f;
                labelZoom.Image = Resources.ZoomOut_16x;
                labelZoomValue.Text = "x10";
                zoomToolStripMenuItem.Text = "Zoom Out";
            }
            else
            {
                ZoomValue = 1 / 10f;
                labelZoom.Image = Resources.ZoomIn_16x;
                labelZoomValue.Text = "x1";
                zoomToolStripMenuItem.Text = "Zoom In";
            }
            canvasPanel.Invalidate();
            GraphicsCanvas.ScaleTransform(ZoomValue, ZoomValue);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = Cursor.Position;
            if (Zoomed)
            {//ToDo...
                point.X = (int)(point.X / ZoomValue);
                point.Y = (int)(point.Y / ZoomValue);
            }
            CurrentPoint = canvasPanel.PointToClient(point);
            if (MouseClickHolded)
            {
                DrawRectangle();
                ValidSize = true;
            }
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = Cursor.Position;
            if (Zoomed)
            {//ToDo...
                point.X = (int)(point.X / ZoomValue);
                point.Y = (int)(point.Y / ZoomValue);
            }
            MouseDownPoint = canvasPanel.PointToClient(point);
            MouseClickHolded = true;
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (ValidSize)
                AddRectangle();
            GraphicsCanvas.Clear(Color.White);
            LoadRectangles();
            MouseClickHolded = false;
            ValidSize = false;
        }

        private void DrawRectangle()
        {
            GraphicsCanvas.Clear(Color.White);
            //Draw previous rectangles
            LoadRectangles();
            Point locationPoint = new Point(Math.Min(MouseDownPoint.X, CurrentPoint.X), Math.Min(MouseDownPoint.Y, CurrentPoint.Y));
            int width = Math.Abs(MouseDownPoint.X - CurrentPoint.X);
            int height = Math.Abs(MouseDownPoint.Y - CurrentPoint.Y);
            Rectangle = new Rectangle(locationPoint.X, locationPoint.Y, width, height);
            GraphicsCanvas.FillRectangle(Brush, Rectangle);
            GraphicsCanvas.DrawRectangle(BorderPen, Rectangle);
        }

        private void LoadRectangles()
        {
            foreach (Rectangle rectangle in Rectangles)
            {
                GraphicsCanvas.FillRectangle(Brush, rectangle);
                GraphicsCanvas.DrawRectangle(BorderPen, rectangle);
            }
        }

        private void canvasPanel_Paint(object sender, PaintEventArgs e)
        {
            GraphicsCanvas.Clear(Color.White);
            LoadRectangles();
            label1.Text = "Cantidad de rectángulos: " + Rectangles.Count;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RectangleBusiness rectangleBusiness = new RectangleBusiness();
            rectangleBusiness.Store(Rectangles);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RectangleBusiness rectangleBusiness = new RectangleBusiness();
            Rectangles = rectangleBusiness.Retrieve();
            canvasPanel.Invalidate();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            Rectangles.Clear();
            canvasPanel.Invalidate();
        }

        private Boolean AddRectangle()
        {
            Boolean rectangleAdded = true;
            Rectangle tempRectangle = new Rectangle();
            LinkedList<Rectangle> mergedRectangles = new LinkedList<Rectangle>();
            foreach (Rectangle listRectangle in Rectangles)
            {
                if (Rectangle.IntersectsWith(listRectangle))
                {
                    MessageBox.Show("Rectangle overlapses with other...");
                    rectangleAdded = false;
                    return false;
                }
                else
                {
                    //If rectangle has common borders with others and can be merged
                    if (InAcceptableRange(Rectangle.Left, listRectangle.Left) && InAcceptableRange(Rectangle.Bottom, listRectangle.Top) && InAcceptableRange(Rectangle.Right, listRectangle.Right))
                    {
                        //MessageBox.Show("On top of another");
                        Rectangle.Height = Rectangle.Height + listRectangle.Height;
                        tempRectangle = listRectangle;
                        mergedRectangles.AddLast(listRectangle);
                    }
                    else if (InAcceptableRange(Rectangle.Right, listRectangle.Right) && InAcceptableRange(Rectangle.Top, listRectangle.Bottom) && InAcceptableRange(Rectangle.Left, listRectangle.Left))
                    {
                        //MessageBox.Show("On bottom of another");
                        Rectangle.Location = listRectangle.Location;
                        Rectangle.Height = Rectangle.Height + listRectangle.Height;
                        tempRectangle = listRectangle;
                        mergedRectangles.AddLast(listRectangle);
                    }
                    else if (InAcceptableRange(Rectangle.Left, listRectangle.Right) && InAcceptableRange(Rectangle.Top, listRectangle.Top) && InAcceptableRange(Rectangle.Bottom, listRectangle.Bottom))
                    {
                        //MessageBox.Show("On right of another");
                        Rectangle.Location = listRectangle.Location;
                        Rectangle.Width = Rectangle.Width + listRectangle.Width;
                        tempRectangle = listRectangle;
                        mergedRectangles.AddLast(listRectangle);
                    }
                    else if (InAcceptableRange(Rectangle.Right, listRectangle.Left) && InAcceptableRange(Rectangle.Top, listRectangle.Top) && InAcceptableRange(Rectangle.Bottom, listRectangle.Bottom))
                    {
                        //MessageBox.Show("On left of another");
                        Rectangle.Width = Rectangle.Width + listRectangle.Width;
                        tempRectangle = listRectangle;
                        mergedRectangles.AddLast(listRectangle);
                    }
                }
            }
            if (rectangleAdded)
            {
                //Rectangles.Remove(tempRectangle);
                foreach (Rectangle mergedRectangle in mergedRectangles)
                {
                    Rectangles.Remove(mergedRectangle);
                }
                Rectangles.AddLast(Rectangle);
                label1.Text = "Cantidad de rectángulos: " + Rectangles.Count;
                return true;

            }
            return false;
        }

        private bool InAcceptableRange(int numberToCheck, int compareTo)
        {
            int acceptedVariation = 5;
            int bottom = compareTo - acceptedVariation;
            int top = compareTo + acceptedVariation;
            return (numberToCheck >= bottom && numberToCheck <= top);
        }
    }
}
