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

        public MainForm()
        {
            InitializeComponent();
            Rectangles = new LinkedList<Rectangle>();
            GraphicsCanvas = this.canvasPanel.CreateGraphics();
            GraphicsCanvas.InterpolationMode = InterpolationMode.HighQualityBilinear;
            BorderPen = new Pen(Color.Olive, 1);
            Brush = new SolidBrush(Color.FromArgb(128, 0, 0, 255));
        }

        private void buttonZoom_Click(object sender, EventArgs e)
        {
            Zoomed = !Zoomed;
            if (Zoomed)
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
            CurrentPoint = canvasPanel.PointToClient(Cursor.Position);
            if (MouseClickHolded)
            {
                DrawRectangle();
                ValidSize = true;
            }
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            MouseDownPoint = canvasPanel.PointToClient(Cursor.Position);
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

        private void buttonClear_Click(object sender, EventArgs e)
        {
            Rectangles.Clear();
            GraphicsCanvas.Clear(Color.White);
            label1.Text = "Cantidad de rectángulos: " + Rectangles.Count;
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private Boolean AddRectangle()
        {
            Boolean rectangleAdded = true;
            Rectangle tempRectangle = new Rectangle();
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
                        MessageBox.Show("On top of another");
                        Rectangle.Height = Rectangle.Height + listRectangle.Height;
                        tempRectangle = listRectangle;
                    }
                    else if (InAcceptableRange(Rectangle.Right, listRectangle.Right) && InAcceptableRange(Rectangle.Top, listRectangle.Bottom) && InAcceptableRange(Rectangle.Left, listRectangle.Left))
                    {
                        MessageBox.Show("On bottom of another");
                        Rectangle.Location = listRectangle.Location;
                        Rectangle.Height = Rectangle.Height + listRectangle.Height;
                        tempRectangle = listRectangle;
                    }
                    else if (InAcceptableRange(Rectangle.Left, listRectangle.Right) && InAcceptableRange(Rectangle.Top, listRectangle.Top) && InAcceptableRange(Rectangle.Bottom, listRectangle.Bottom))
                    {
                        MessageBox.Show("On right of another");
                        Rectangle.Location = listRectangle.Location;
                        Rectangle.Width = Rectangle.Width + listRectangle.Width;
                        tempRectangle = listRectangle;
                    }
                    else if (InAcceptableRange(Rectangle.Right, listRectangle.Left) && InAcceptableRange(Rectangle.Top, listRectangle.Top) && InAcceptableRange(Rectangle.Bottom, listRectangle.Bottom))
                    {
                        MessageBox.Show("On left of another");
                        Rectangle.Width = Rectangle.Width + listRectangle.Width;
                        tempRectangle = listRectangle;
                    }
                }
            }
            if (rectangleAdded)
            {
                Rectangles.Remove(tempRectangle);
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

        private bool MergeIfPossible(Rectangle comparableRectangle)
        {

            return false;
        }
    }
}
