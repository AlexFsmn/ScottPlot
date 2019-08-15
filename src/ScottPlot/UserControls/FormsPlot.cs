﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace ScottPlot
{
    public partial class FormsPlot : UserControl
    {
        public Plot plt = new Plot();
        private bool currentlyRendering = false;
        private System.Timers.Timer timer;

        public FormsPlot()
        {
            InitializeComponent();
            timer = new System.Timers.Timer() // create before first render call
            {
                AutoReset = false,
                SynchronizingObject = this,
                Enabled = false,
            };
            timer.Elapsed += (o, arg) => Render(skipIfCurrentlyRendering: false);
            plt.Style(ScottPlot.Style.Control);
            pbPlot.MouseWheel += PbPlot_MouseWheel;
            if (Process.GetCurrentProcess().ProcessName == "devenv")
                ScottPlot.Tools.DesignerModeDemoPlot(plt);
            PbPlot_SizeChanged(null, null);
        }

        public void Render(bool skipIfCurrentlyRendering = false, bool lowQuality = false)
        {
            if (timer.Enabled) // at any reason stop timer, it always set correct after render call
                timer.Stop();
            if (!(skipIfCurrentlyRendering && currentlyRendering))
            {
                currentlyRendering = true;
                pbPlot.Image = plt.GetBitmap(true, lowQuality);
                if (plt.mouseTracker.IsDraggingSomething())
                    Application.DoEvents();
                currentlyRendering = false;
            }
        }

        private void LaunchMenu()
        {
            plt.GetSettings().mouseIsPanning = false;
            plt.GetSettings().mouseIsZooming = false;

            var frm = new UserControls.FormSettings(plt);
            frm.ShowDialog();
        }

        private void PbPlot_SizeChanged(object sender, EventArgs e)
        {
            plt.Resize(Width, Height);
            Render(skipIfCurrentlyRendering: false);
        }

        private void PbPlot_MouseDown(object sender, MouseEventArgs e)
        {
            plt.mouseTracker.MouseDown(e.Location);
            if (plt.mouseTracker.PlottableUnderCursor(e.Location) != null)
                OnMouseDownOnPlottable(EventArgs.Empty);
        }

        private void PbPlot_MouseMove(object sender, MouseEventArgs e)
        {
            plt.mouseTracker.MouseMove(e.Location);
            OnMouseMoved(EventArgs.Empty);

            var plottableUnderCursor = plt.mouseTracker.PlottableUnderCursor(e.Location);
            if (plottableUnderCursor != null)
            {
                if (e.Button != MouseButtons.None)
                    OnMouseDragPlottable(EventArgs.Empty);
                if (plottableUnderCursor is PlottableAxLine axLine)
                {
                    if (axLine.vertical == true)
                        pbPlot.Cursor = Cursors.SizeWE;
                    else
                        pbPlot.Cursor = Cursors.SizeNS;
                }
            }
            else
            {
                pbPlot.Cursor = Cursors.Arrow;
            }

            if (e.Button != MouseButtons.None)
            {
                Render(skipIfCurrentlyRendering: true, lowQuality: plt.mouseTracker.lowQualityWhileInteracting);
                OnMouseDragged(EventArgs.Empty);
            }
        }

        private void PbPlot_MouseUp(object sender, MouseEventArgs e)
        {
            if (!plt.mouseTracker.MouseHasMoved())
            {
                LaunchMenu();
                return;
            }

            plt.mouseTracker.MouseUp(e.Location);
            if (plt.mouseTracker.lowQualityWhileInteracting && plt.mouseTracker.mouseUpHQRenderDelay > 0)
            {
                Render(false, true); // LQ render
                timer.Interval = plt.mouseTracker.mouseUpHQRenderDelay;
                timer.Start(); // set delay for HQ render on MouseUp
            }
            else // HQ render at LQ - off, or mouseUpRenderDelay == 0
                Render(skipIfCurrentlyRendering: false);
            if (plt.mouseTracker.PlottableUnderCursor(e.Location) != null)
                OnMouseDropPlottable(EventArgs.Empty);
        }

        private void PbPlot_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                plt.AxisAuto();
                Render(skipIfCurrentlyRendering: false);
            }
            else if (e.Button == MouseButtons.Right && plt.mouseTracker.mouseDownStopwatch.ElapsedMilliseconds < 100)
            {
                LaunchMenu();
            }
        }

        private void PbPlot_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            plt.Benchmark(toggle: true);
            Render(skipIfCurrentlyRendering: false);
        }

        private void PbPlot_MouseWheel(object sender, MouseEventArgs e)
        {
            double zoomAmount = 0.15;
            PointF zoomCenter = plt.CoordinateFromPixel(e.Location);
            if (e.Delta > 0)
                plt.AxisZoom(1 + zoomAmount, 1 + zoomAmount, zoomCenter);
            else
                plt.AxisZoom(1 - zoomAmount, 1 - zoomAmount, zoomCenter);

            if (plt.mouseTracker.lowQualityWhileInteracting && plt.mouseTracker.mouseWheelHQRenderDelay > 0)
            {
                Render(false, true);
                timer.Interval = plt.mouseTracker.mouseWheelHQRenderDelay; // delay in ms
                timer.Start();
            }
            else
                Render(skipIfCurrentlyRendering: false);
        }

        public event EventHandler MouseDownOnPlottable;
        public event EventHandler MouseDragPlottable;
        public event EventHandler MouseMoved;
        public event EventHandler MouseDragged;
        public event EventHandler MouseDropPlottable;
        protected virtual void OnMouseDownOnPlottable(EventArgs e) { MouseDownOnPlottable?.Invoke(this, e); }
        protected virtual void OnMouseDragPlottable(EventArgs e) { MouseDragPlottable?.Invoke(this, e); }
        protected virtual void OnMouseMoved(EventArgs e) { MouseMoved?.Invoke(this, e); }
        protected virtual void OnMouseDragged(EventArgs e) { MouseDragged?.Invoke(this, e); }
        protected virtual void OnMouseDropPlottable(EventArgs e) { MouseDropPlottable?.Invoke(this, e); }
    }
}