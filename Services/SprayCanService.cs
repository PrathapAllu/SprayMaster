using PropertyChanged;
using SprayMaster.Helpers;
using SprayMaster.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SprayMaster.Services
{
    [AddINotifyPropertyChangedInterface]
    public class SprayCanService : ISprayCanService
    {
        private InkCanvas inkCanvas;
        private readonly ToolManager toolManager;
        private readonly DispatcherTimer sprayTimer;
        private readonly Random random = new();
        private bool IsMousePressed;
        private Point sprayCenter;
        public double EraserSize { get; set; } = 5;
        public bool isSprayCanActive { get; set; } = false;

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

            if (toolManager.isUseEraser && IsMousePressed)
            {
                EraseElements(sprayCenter);
            }
        }

        private void EraseElements(Point position)
        {
            var elementsToRemove = inkCanvas.Children
                .OfType<Ellipse>()
                .Where(ellipse =>
                {
                    Point ellipsePos = new(
                        InkCanvas.GetLeft(ellipse) + ellipse.Width / 2,
                        InkCanvas.GetTop(ellipse) + ellipse.Height / 2
                    );
                    return CalculateDistance(position, ellipsePos) < EraserSize;
                })
                .ToList();

            foreach (var element in elementsToRemove)
            {
                inkCanvas.Children.Remove(element);
            }

            inkCanvas.Strokes.Erase(new Point[] { position }, new EllipseStylusShape(EraserSize, EraserSize));
        }

        private double CalculateDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        private void SprayTimer_Tick(object sender, EventArgs e)
        {
            if (!IsMousePressed || !isSprayCanActive) return;

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


