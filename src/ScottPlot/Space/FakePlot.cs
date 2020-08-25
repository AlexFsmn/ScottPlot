﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ScottPlot.Space
{
    [Obsolete("delete this module after the new rendering system is finished")]
    public static class FakePlot
    {
        public static Bitmap Triangle(FigureInfo fig)
        {
            var bmp = new Bitmap((int)fig.Width, (int)fig.Height);
            using (var gfx = Graphics.FromImage(bmp))
            using (var fillBrush = new SolidBrush(Color.FromArgb(50, Color.Black)))
            using (var linePen = new Pen(Color.Blue, 3))
            {
                gfx.Clear(Color.White);
                gfx.FillRectangle(fillBrush, fig.DataOffsetX, fig.DataOffsetY, fig.DataWidth, fig.DataHeight);

                // simuate a plottable (a blue triangle)
                List<PointF> points = new List<PointF>();
                points.Add(new PointF(fig.GetPixelX(-.8), fig.GetPixelY(8)));
                points.Add(new PointF(fig.GetPixelX(-.8), fig.GetPixelY(-8)));
                points.Add(new PointF(fig.GetPixelX(.8), fig.GetPixelY(8)));
                points.Add(new PointF(fig.GetPixelX(-.8), fig.GetPixelY(8)));
                gfx.DrawLines(linePen, points.ToArray());
            }
            return bmp;
        }
    }
}