using PropertyChanged;
using SprayMaster.Helpers;
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
            var window = Application.Current.MainWindow as MainWindow;
            this.inkCanvas = window?.canvasPanel;
            window.CanvasMouseEvent += OnCanvasMouseEvent;
            this.inkCanvas = inkCanvas;
            this.sprayRadius = sprayRadius;
            this.particlesPerSecond = (int)(100000 * sprayRadius);
            this.random = new Random();
            this.sprayAttributes = sprayAttributes;

            this.sprayTimer = new DispatcherTimer();
            this.sprayTimer.Tick += SprayTimer_Tick;
        }

        private void OnCanvasMouseEvent(object sender, CanvasMouseEventArgs e)
        {
            IsMousePressed = e.IsPressed;
            sprayCenter = e.Position;

            if (!e.IsPressed)
                inkCanvas.Cursor = Cursors.Arrow;
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
    }
}


