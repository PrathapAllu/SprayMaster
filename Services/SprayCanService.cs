using PropertyChanged;
using SprayMaster.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SprayMaster.Services
{
    [AddINotifyPropertyChangedInterface]
    public class SprayCanService
    {
        private InkCanvas inkCanvas;
        private readonly ToolManager toolManager;
        private readonly DispatcherTimer sprayTimer;
        private readonly Random random = new();
        private bool IsMousePressed;
        private Point sprayCenter;

        public SprayCanService(ToolManager toolManager)
        {
            this.toolManager = toolManager;
            this.sprayTimer = new DispatcherTimer();
            this.sprayTimer.Tick += SprayTimer_Tick;
        }

        public void Initialize(InkCanvas canvas)
        {
            inkCanvas = canvas;
        }

        public void OnCanvasMouseEvent(object sender, CanvasMouseEventArgs e)
        {
            IsMousePressed = e.IsPressed;
            sprayCenter = e.Position;
        }

        private void SprayTimer_Tick(object sender, EventArgs e)
        {
            if (!IsMousePressed || !toolManager.isSprayCanActive) return;

            for (int i = 0; i < 5; i++)
            {
                double angle = random.NextDouble() * 2 * Math.PI;
                double distance = Math.Sqrt(random.NextDouble()) * toolManager.BrushSize;
                double particleX = sprayCenter.X + distance * Math.Cos(angle);
                double particleY = sprayCenter.Y + distance * Math.Sin(angle);

                Ellipse ellipse = new()
                {
                    Width = toolManager.DrawingAttributes.Width,
                    Height = toolManager.DrawingAttributes.Height,
                    Fill = new SolidColorBrush(toolManager.SelectedColor),
                    Opacity = toolManager.SelectedColor.A / 255.0,
                };

                InkCanvas.SetLeft(ellipse, particleX - toolManager.DrawingAttributes.Width / 2);
                InkCanvas.SetTop(ellipse, particleY - toolManager.DrawingAttributes.Height / 2);
                inkCanvas.Children.Add(ellipse);
            }
        }

        public void StartSpraying()
        {
            sprayTimer.Interval = TimeSpan.FromSeconds(1.0 / (100000 * toolManager.BrushSize));
            sprayTimer.Start();
        }

        public void StopSpraying()
        {
            sprayTimer.Stop();
        }
    }
}


