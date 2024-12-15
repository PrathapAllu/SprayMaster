﻿using PropertyChanged;
using SprayMaster.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SprayMaster.Services
{
    [AddINotifyPropertyChangedInterface]
    public class SprayCanService
    {
        private readonly InkCanvas inkCanvas;
        private readonly double sprayRadius;
        private readonly int particlesPerSecond;
        private readonly DispatcherTimer sprayTimer;
        private readonly Random random;
        private readonly DrawingAttributes sprayAttributes;
        private bool IsMousePressed = false;
        private Point sprayCenter;

        public SprayCanService(double sprayRadius, DrawingAttributes sprayAttributes)
        {
            //TODO:Inject MainWindow via DI instead of accessing like this
            var inkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
            this.inkCanvas = inkCanvas;
            this.sprayRadius = sprayRadius;
            this.particlesPerSecond = (int)(100000 * sprayRadius);
            this.random = new Random();
            this.sprayAttributes = sprayAttributes;

            this.sprayTimer = new DispatcherTimer();
            this.sprayTimer.Tick += SprayTimer_Tick;

            // Subscribe to mouse events
            inkCanvas.MouseDown += InkCanvas_MouseDown;
            inkCanvas.MouseMove += InkCanvas_MouseMove;
            inkCanvas.MouseUp += InkCanvas_MouseUp;
        }

        public void StartSpraying()
        {
            sprayTimer.Interval = TimeSpan.FromSeconds(1.0 / particlesPerSecond);
            sprayTimer.Start();
        }

        public void StopSpraying()
        {
            sprayTimer.Stop();
        }

        private void SprayTimer_Tick(object sender, EventArgs e)
        {
            if (IsMousePressed)
            {
                for (int i = 0; i < 5; i++)
                {
                    double angle = random.NextDouble() * 2 * Math.PI;
                    double distance = Math.Sqrt(random.NextDouble()) * sprayRadius;

                    double particleX = sprayCenter.X + distance * Math.Cos(angle);
                    double particleY = sprayCenter.Y + distance * Math.Sin(angle);

                    Ellipse ellipse = new Ellipse
                    {
                        Width = sprayAttributes.Width,
                        Height = sprayAttributes.Height,
                        Fill = new SolidColorBrush(sprayAttributes.Color),
                        Opacity = sprayAttributes.Color.A / 255.0,
                    };

                    InkCanvas.SetLeft(ellipse, particleX - sprayAttributes.Width / 2);
                    InkCanvas.SetTop(ellipse, particleY - sprayAttributes.Height / 2);

                    inkCanvas.Children.Add(ellipse);
                }
            }
        }
        #region Canvas Mouse events regin
        private void InkCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsMousePressed = true;
            sprayCenter = e.GetPosition(inkCanvas);
        }

        private void InkCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMousePressed)
            {
                sprayCenter = e.GetPosition(inkCanvas);
            }
        }

        private void InkCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsMousePressed = false;
            inkCanvas.Cursor = Cursors.Arrow;
        }
        #endregion
    }
}


