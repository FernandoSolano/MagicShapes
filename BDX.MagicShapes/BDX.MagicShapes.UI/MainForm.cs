using BDX.MagicShapes.Core.Business;
using BDX.MagicShapes.Core.Model;
using BDX.MagicShapes.UI.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace BDX.MagicShapes.UI
{
    public partial class MainForm : Form
    {
        private Point CurrentPoint, MouseDownPoint;
        private bool MouseClickHolded, ValidSize, Zoomed;
        private Graphics GraphicsCanvas;
        private Pen BorderPen;
        private SolidBrush Brush, AuxBrush;
        private Rectangle Rectangle;
        private LinkedList<Rectangle> Rectangles, AuxRectangles;
        private LinkedList<Point[]> AuxLines;
        private AppState PreviousState, LatestState;
        private float ZoomValue;
        private int mergedCounter;

        public MainForm()
        {
            InitializeComponent();
            Rectangles = new LinkedList<Rectangle>();
            AuxRectangles = new LinkedList<Rectangle>();
            AuxLines = new LinkedList<Point[]>();
            PreviousState = new AppState();
            LatestState = new AppState();
            GraphicsCanvas = this.canvasPanel.CreateGraphics();
            GraphicsCanvas.InterpolationMode = InterpolationMode.HighQualityBilinear;
            ZoomValue = 1f;
            Zoomed = false;
            BorderPen = new Pen(Color.Olive, 1);
            Brush = new SolidBrush(Color.FromArgb(128, 0, 0, 255));
            AuxBrush = new SolidBrush(ColorTranslator.FromHtml("#7F7FFF"));
            mergedCounter = 0;
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
            foreach (Rectangle rectangle in AuxRectangles)
            {
                //GraphicsCanvas.FillRectangle(Brush, rectangle);
                GraphicsCanvas.FillRectangle(AuxBrush, rectangle);
            }
        }

        private void canvasPanel_Paint(object sender, PaintEventArgs e)
        {
            GraphicsCanvas.Clear(Color.White);
            LoadRectangles();
            label1.Text = "Shapes quantity: " + (Rectangles.Count - mergedCounter);
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LatestState.Rectangles1 = Rectangles;
            LatestState.AuxRectangles1 = AuxRectangles;
            Rectangles = PreviousState.Rectangles1;
            AuxRectangles = PreviousState.AuxRectangles1;
            canvasPanel.Invalidate();
            redoToolStripMenuItem.Enabled = true;
            undoToolStripMenuItem.Enabled = false;
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreviousState.Rectangles1 = Rectangles;
            PreviousState.AuxRectangles1 = AuxRectangles;
            Rectangles = LatestState.Rectangles1;
            AuxRectangles = LatestState.AuxRectangles1;
            canvasPanel.Invalidate();
            redoToolStripMenuItem.Enabled = false;
            undoToolStripMenuItem.Enabled = true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            saveFileDialog.FileName = "MagicShapesSave";
            saveFileDialog.DefaultExt = "bin";
            saveFileDialog.Filter = "Binary (*.bin)|*.bin";
            saveFileDialog.ShowDialog();
            RectangleBusiness rectangleBusiness = new RectangleBusiness();
            rectangleBusiness.Store(new AppState(Rectangles, AuxRectangles), saveFileDialog.FileName);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            openFileDialog.Filter = "Binary (*.bin)|*.bin";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                RectangleBusiness rectangleBusiness = new RectangleBusiness();
                AppState appState = new AppState();
                appState = rectangleBusiness.Retrieve(openFileDialog.FileName);
                Rectangles = appState.Rectangles1;
                AuxRectangles = appState.AuxRectangles1;
                canvasPanel.Invalidate();
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            PreviousState.Rectangles1 = Rectangles;
            PreviousState.AuxRectangles1 = AuxRectangles;
            Rectangles.Clear();
            AuxRectangles.Clear();
            LatestState.Rectangles1 = Rectangles;
            LatestState.AuxRectangles1 = AuxRectangles;
            mergedCounter = 0;
            canvasPanel.Invalidate();
            redoToolStripMenuItem.Enabled = false;
            undoToolStripMenuItem.Enabled = true;
        }

        public static T DeepCopy<T>(T other)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, other);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }

        private Boolean AddRectangle()
        {
            PreviousState.Rectangles1 = DeepCopy(Rectangles);
            PreviousState.AuxRectangles1 = DeepCopy(AuxRectangles);
            Boolean rectangleAdded = true;
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
                        mergedRectangles.AddLast(listRectangle);
                    }
                    else if (InAcceptableRange(Rectangle.Right, listRectangle.Right) && InAcceptableRange(Rectangle.Top, listRectangle.Bottom) && InAcceptableRange(Rectangle.Left, listRectangle.Left))
                    {
                        //MessageBox.Show("On bottom of another");
                        Rectangle.Location = listRectangle.Location;
                        Rectangle.Height = Rectangle.Height + listRectangle.Height;
                        mergedRectangles.AddLast(listRectangle);
                    }
                    else if (InAcceptableRange(Rectangle.Left, listRectangle.Right) && InAcceptableRange(Rectangle.Top, listRectangle.Top) && InAcceptableRange(Rectangle.Bottom, listRectangle.Bottom))
                    {
                        //MessageBox.Show("On right of another");
                        Rectangle.Location = listRectangle.Location;
                        Rectangle.Width = Rectangle.Width + listRectangle.Width;
                        mergedRectangles.AddLast(listRectangle);
                    }
                    else if (InAcceptableRange(Rectangle.Right, listRectangle.Left) && InAcceptableRange(Rectangle.Top, listRectangle.Top) && InAcceptableRange(Rectangle.Bottom, listRectangle.Bottom))
                    {
                        //MessageBox.Show("On left of another");
                        Rectangle.Width = Rectangle.Width + listRectangle.Width;
                        mergedRectangles.AddLast(listRectangle);
                    }
                    else//Special cases when the new rectangle has different width or height
                    {
                        Rectangle auxRectangle = new Rectangle();
                        if (InAcceptableRange(Rectangle.Bottom, listRectangle.Top))
                        {
                            if (Rectangle.Width > listRectangle.Width && listRectangle.Right <= Rectangle.Right && listRectangle.Left >= Rectangle.Left)
                            {
                                auxRectangle.Location = new Point(listRectangle.X, Rectangle.Top + Rectangle.Height / 2);
                                auxRectangle.Size = new Size(listRectangle.Width, (Rectangle.Height + listRectangle.Height) / 2);
                                AuxRectangles.AddLast(auxRectangle);
                                mergedCounter++;
                            }
                            else if (Rectangle.Width < listRectangle.Width && listRectangle.Right >= Rectangle.Right && listRectangle.Left <= Rectangle.Left)
                            {
                                auxRectangle.Location = new Point(Rectangle.X, Rectangle.Top + Rectangle.Height / 2);
                                auxRectangle.Size = new Size(Rectangle.Width, (Rectangle.Height + listRectangle.Height) / 2);
                                AuxRectangles.AddLast(auxRectangle);
                                mergedCounter++;
                            }
                        }
                        else if (InAcceptableRange(Rectangle.Top, listRectangle.Bottom))
                        {
                            if (Rectangle.Width > listRectangle.Width && listRectangle.Right <= Rectangle.Right && listRectangle.Left >= Rectangle.Left)
                            {
                                auxRectangle.Location = new Point(listRectangle.X, Rectangle.Top - listRectangle.Height / 2);
                                auxRectangle.Size = new Size(listRectangle.Width, (Rectangle.Height + listRectangle.Height) / 2);
                                AuxRectangles.AddLast(auxRectangle);
                                mergedCounter++;
                            }
                            else if (Rectangle.Width < listRectangle.Width && listRectangle.Right >= Rectangle.Right && listRectangle.Left <= Rectangle.Left)
                            {
                                auxRectangle.Location = new Point(Rectangle.X, Rectangle.Top - listRectangle.Height / 2);//************
                                auxRectangle.Size = new Size(Rectangle.Width, (Rectangle.Height + listRectangle.Height) / 2);
                                AuxRectangles.AddLast(auxRectangle);
                                mergedCounter++;
                            }
                        }//If the new rectangle is on side of another...
                        else if (InAcceptableRange(Rectangle.Right, listRectangle.Left))
                        {
                            if (Rectangle.Height > listRectangle.Height && listRectangle.Bottom <= Rectangle.Bottom && listRectangle.Top >= Rectangle.Top)
                            {
                                auxRectangle.Location = new Point(listRectangle.X - Rectangle.Width / 2, listRectangle.Y);
                                auxRectangle.Size = new Size((Rectangle.Width + listRectangle.Width) / 2, listRectangle.Height);
                                AuxRectangles.AddLast(auxRectangle);
                                mergedCounter++;
                            }
                            else if (Rectangle.Height < listRectangle.Height && listRectangle.Bottom >= Rectangle.Bottom && listRectangle.Top <= Rectangle.Top)
                            {
                                auxRectangle.Location = new Point(listRectangle.X - Rectangle.Width / 2, Rectangle.Y);
                                auxRectangle.Size = new Size((Rectangle.Width + listRectangle.Width) / 2, Rectangle.Height);
                                AuxRectangles.AddLast(auxRectangle);
                                mergedCounter++;
                            }
                        }
                        else if (InAcceptableRange(Rectangle.Left, listRectangle.Right))
                        {
                            if (Rectangle.Height > listRectangle.Height && listRectangle.Bottom <= Rectangle.Bottom && listRectangle.Top >= Rectangle.Top)
                            {
                                auxRectangle.Location = new Point((listRectangle.Right + listRectangle.Left) / 2, listRectangle.Y);
                                auxRectangle.Size = new Size((Rectangle.Width + listRectangle.Width) / 2, listRectangle.Height);
                                AuxRectangles.AddLast(auxRectangle);
                                mergedCounter++;
                            }
                            else if (Rectangle.Height < listRectangle.Height && listRectangle.Bottom >= Rectangle.Bottom && listRectangle.Top <= Rectangle.Top)
                            {
                                auxRectangle.Location = new Point((listRectangle.Right + listRectangle.Left) / 2, Rectangle.Y);
                                auxRectangle.Size = new Size((Rectangle.Width + listRectangle.Width) / 2, Rectangle.Height);
                                AuxRectangles.AddLast(auxRectangle);
                                mergedCounter++;
                            }
                        }
                    }
                }
            }
            if (rectangleAdded)
            {
                foreach (Rectangle mergedRectangle in mergedRectangles)
                {
                    Rectangles.Remove(mergedRectangle);
                }
                Rectangles.AddLast(Rectangle);
                label1.Text = "Shapes quantity: " + (Rectangles.Count - mergedCounter);
                undoToolStripMenuItem.Enabled = true;
                redoToolStripMenuItem.Enabled = false;
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
