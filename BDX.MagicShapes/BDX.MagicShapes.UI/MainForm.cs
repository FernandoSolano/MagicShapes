using System;
using System.Drawing;
using System.Windows.Forms;

namespace BDX.MagicShapes.UI
{
    public partial class MainForm : Form
    {
        private Point currentPoint, mouseDownPoint, mouseUpPoint;
        private bool zoomed;

        public MainForm()
        {
            InitializeComponent();
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
            currentPoint = canvas.PointToClient(Cursor.Position);
            //ToDo... Here the rectangle should be temporarly drawed but only when mouse button is hold
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDownPoint = canvas.PointToClient(Cursor.Position);
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            mouseUpPoint = canvas.PointToClient(Cursor.Position);
            drawRectangle();
        }

        private void drawRectangle()
        {
            Point locationPoint = new Point(Math.Min(mouseDownPoint.X, mouseUpPoint.X), Math.Min(mouseDownPoint.Y, mouseUpPoint.Y));
            Graphics graphics = canvas.CreateGraphics();
            Pen pen = new Pen(Color.Olive, 2);
            SolidBrush brush = new SolidBrush(Color.FromArgb(128, 0, 0, 255));
            int width = Math.Abs(mouseDownPoint.X - mouseUpPoint.X);
            int height = Math.Abs(mouseDownPoint.Y - mouseUpPoint.Y);
            Rectangle rectangle = new Rectangle(locationPoint.X, locationPoint.Y, width, height);
            graphics.FillRectangle(brush, rectangle);
            graphics.DrawRectangle(pen, rectangle);
        }
    }
}
