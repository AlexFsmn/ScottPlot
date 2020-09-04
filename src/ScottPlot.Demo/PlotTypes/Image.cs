﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace ScottPlot.Demo.PlotTypes
{
    class Image
    {
        public class Quickstart : PlotDemo, IPlotDemo
        {
            public string name { get; } = "Image Quickstart";
            public string description { get; } = "Images can be placed at any X/Y location and styled using arguments.";

            public void Render(Plot plt)
            {
                int pointCount = 51;
                double[] x = DataGen.Consecutive(pointCount);
                double[] sin = DataGen.Sin(pointCount);
                double[] cos = DataGen.Cos(pointCount);

                plt.PlotScatter(x, sin);
                plt.PlotScatter(x, cos);

                Bitmap image = new Bitmap("Images/niceBackground.bmp");
                plt.PlotImage(image, 0, 0);
            }
        }

        public class Alignment : PlotDemo, IPlotDemo
        {
            public string name { get; } = "Image Alignment";
            public string description { get; } = "Image alignment and rotation can be customized using arguments.";

            private Bitmap image = new Bitmap("Images/niceBackground.bmp");
            public void Render(Plot plt)
            {
                int pointCount = 51;
                double[] x = DataGen.Consecutive(pointCount);
                double[] sin = DataGen.Sin(pointCount);
                double[] cos = DataGen.Cos(pointCount);

                plt.PlotScatter(x, sin);
                plt.PlotScatter(x, cos);

                plt.PlotImage(image, 5, 0.8);
                plt.PlotPoint(5, 0.8, color: Color.Green);

                plt.PlotImage(image, 20, 0.3);
                plt.PlotPoint(20, 0.3, color: Color.Black, markerSize: 15);

                plt.PlotImage(image, 30, 0, alignment: TextAlignment.middleCenter);
                plt.PlotPoint(30, 0, color: Color.Black, markerSize: 15);

                plt.PlotImage(image, 30, -0.3, alignment: TextAlignment.upperLeft);
                plt.PlotPoint(30, -0.3, color: Color.Black, markerSize: 15);

                plt.PlotImage(image, 5, -.5, rotation: -30);
                plt.PlotPoint(5, -.5, color: Color.Blue, markerSize: 15);

                plt.PlotImage(image, 15, -.6, frame: true, frameColor: Color.DarkRed, frameSize: 25);
            }
        }
    }
}