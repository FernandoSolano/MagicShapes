﻿using BDX.MagicShapes.Core.Business;
using BDX.MagicShapes.Core.Model;
using BDX.MagicShapes.UI.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace BDX.MagicShapes.UI
{
    public partial class MainForm : Form
    {
        private Point CurrentPoint, MouseDownPoint;
        private bool MouseClickHolded, ValidSize, Zoomed;
        private Graphics GraphicsCanvas;
        private Pen BorderPen;
        private SolidBrush FillBrush;
        private float ZoomValue;

        private LinkedList<Polygon> Polygons;
        private Polygon Polygon;

        public MainForm()
        {
            InitializeComponent();
            Polygons = new LinkedList<Polygon>();
            GraphicsCanvas = this.canvasPanel.CreateGraphics();
            GraphicsCanvas.InterpolationMode = InterpolationMode.HighQualityBilinear;
            ZoomValue = 1f;
            Zoomed = false;
            BorderPen = new Pen(Color.Olive, 1);
            FillBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 255));
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            openFileDialog.Filter = "Binary (*.bin)|*.bin";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                PolygonBusiness polygonBusiness = new PolygonBusiness();
                LinkedList<Polygon> filePolygons = polygonBusiness.Retrieve(openFileDialog.FileName);
                Polygons = filePolygons;
                canvasPanel.Invalidate();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            saveFileDialog.FileName = "MagicShapesSave";
            saveFileDialog.DefaultExt = "bin";
            saveFileDialog.Filter = "Binary (*.bin)|*.bin";
            saveFileDialog.ShowDialog();
            PolygonBusiness polygonBusiness = new PolygonBusiness();
            polygonBusiness.Store(Polygons, saveFileDialog.FileName);
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

        private void buttonClear_Click(object sender, EventArgs e)
        {
            Polygons.Clear();
            canvasPanel.Invalidate();
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
                AddPolygon();
            canvasPanel.Invalidate();
            MouseClickHolded = false;
            ValidSize = false;
        }

        //ToDo... Paint external borders only
        private void canvasPanel_Paint(object sender, PaintEventArgs e)
        {
            GraphicsCanvas.Clear(Color.White);
            foreach (Polygon polygon in Polygons)
            {
                GraphicsCanvas.FillPolygon(FillBrush, polygon.Points.ToArray());
                GraphicsCanvas.DrawPolygon(BorderPen, polygon.Points.ToArray());
            }
            label1.Text = "Total shapes: " + Polygons.Count;
        }

        private Boolean AddPolygon()
        {
            Boolean polygonAdded = true;
            Polygon tempPolygon = new Polygon();
            LinkedList<Polygon> mergedPolygons = new LinkedList<Polygon>();
            foreach (Polygon listPolygon in Polygons)
            {
                if (Intersects(listPolygon))
                {
                    MessageBox.Show("Rectangle overlapses with other...");
                    polygonAdded = false;
                    return false;
                }
                else
                {
                    //If rectangle has common borders with others and can be merged
                    if (InAcceptableRange(Polygon.Left, listPolygon.Left) && InAcceptableRange(Polygon.Bottom, listPolygon.Top) && InAcceptableRange(Polygon.Right, listPolygon.Right))
                    {
                        //MessageBox.Show("On top of another");
                        Polygon.Height = Polygon.Height + listPolygon.Height;
                        tempPolygon = listPolygon;
                        mergedPolygons.AddLast(listPolygon);
                    }
                    else if (InAcceptableRange(Polygon.Right, listPolygon.Right) && InAcceptableRange(Polygon.Top, listPolygon.Bottom) && InAcceptableRange(Polygon.Left, listPolygon.Left))
                    {
                        //MessageBox.Show("On bottom of another");
                        Polygon.Location = listPolygon.Location;
                        Polygon.Height = Polygon.Height + listPolygon.Height;
                        tempPolygon = listPolygon;
                        mergedPolygons.AddLast(listPolygon);
                    }
                    else if (InAcceptableRange(Polygon.Left, listPolygon.Right) && InAcceptableRange(Polygon.Top, listPolygon.Top) && InAcceptableRange(Polygon.Bottom, listPolygon.Bottom))
                    {
                        //MessageBox.Show("On right of another");
                        Polygon.Location = listPolygon.Location;
                        Polygon.Width = Polygon.Width + listPolygon.Width;
                        tempPolygon = listPolygon;
                        mergedPolygons.AddLast(listPolygon);
                    }
                    else if (InAcceptableRange(Polygon.Right, listPolygon.Left) && InAcceptableRange(Polygon.Top, listPolygon.Top) && InAcceptableRange(Polygon.Bottom, listPolygon.Bottom))
                    {
                        //MessageBox.Show("On left of another");
                        Polygon.Width = Polygon.Width + listPolygon.Width;
                        tempPolygon = listPolygon;
                        mergedPolygons.AddLast(listPolygon);
                    }
                }
            }
            if (polygonAdded)
            {
                //Rectangles.Remove(tempRectangle);
                foreach (Polygon mergedRectangle in mergedPolygons)
                {
                    Polygons.Remove(mergedRectangle);
                }
                Polygons.AddLast(Polygon);
                label1.Text = "Cantidad de rectángulos: " + Polygons.Count;
                return true;

            }
            return false;
        }

        private void DrawRectangle()
        {
            //Draw previous rectangles
            canvasPanel.Invalidate();
            Point locationPoint = new Point(Math.Min(MouseDownPoint.X, CurrentPoint.X), Math.Min(MouseDownPoint.Y, CurrentPoint.Y));
            int width = Math.Abs(MouseDownPoint.X - CurrentPoint.X);
            int height = Math.Abs(MouseDownPoint.Y - CurrentPoint.Y);
            Polygon = new Polygon();
            Polygon.Points.Add(locationPoint);
            Polygon.Points.Add(new Point(CurrentPoint.X, locationPoint.Y));
            Polygon.Points.Add(CurrentPoint);
            Polygon.Points.Add(new Point(locationPoint.X, CurrentPoint.Y));
            GraphicsCanvas.FillPolygon(FillBrush, Polygon.Points.ToArray());
            GraphicsCanvas.DrawPolygon(BorderPen, Polygon.Points.ToArray());
        }

        private bool Intersects(Polygon listPolygon)
        {
            //ToDo...
            return false;
        }

        private bool InAcceptableRange(int numberToCheck, int compareTo)
        {
            int acceptedVariation = 5;
            int bottom = compareTo - acceptedVariation;
            int top = compareTo + acceptedVariation;
            return (numberToCheck >= bottom && numberToCheck <= top);
        }

        public void DrawPolygon()
        {
            // Create points that define polygon.
            Point point1 = new Point(50, 50);
            Point point2 = new Point(100, 50);
            Point point3 = new Point(100, 100);
            Point point4 = new Point(50, 100);
            Point[] curvePoints =
                     {
                 point1,
                 point2,
                 point3,
                 point4
             };
            // Draw polygon to screen.
            GraphicsCanvas.DrawPolygon(BorderPen, curvePoints);
        }
    }
}
