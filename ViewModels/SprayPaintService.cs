using SprayMaster.Interface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SprayMaster.ViewModels
{

    public class SprayPaintService : ISprayPaintService
    {
        private readonly Random _random = new Random();

        public void SprayAt(Point point, Color color, double density)
        {
            int particles = (int)(density * 10);
            double radius = density / 5;

            for (int i = 0; i < particles; i++)
            {
                double angle = _random.NextDouble() * 2 * Math.PI;
                double distance = _random.NextDouble() * radius;

                double x = point.X + distance * Math.Cos(angle);
                double y = point.Y + distance * Math.Sin(angle);

                var dot = new Ellipse
                {
                    Width = 2,
                    Height = 2,
                    Fill = new SolidColorBrush(color)
                };

                Canvas.SetLeft(dot, x);
                Canvas.SetTop(dot, y);
            }
        }

        public void Erase(Point point, double size, Canvas canvas)
        {
            var hitTest = new List<UIElement>();

            VisualTreeHelper.HitTest(canvas, null,
                result =>
                {
                    if (result.VisualHit is Ellipse)
                        hitTest.Add(result.VisualHit as UIElement);
                    return HitTestResultBehavior.Continue;
                },
                new GeometryHitTestParameters(
                    new EllipseGeometry(point, size, size)));

            foreach (var element in hitTest)
                canvas.Children.Remove(element);
        }
    }
}
