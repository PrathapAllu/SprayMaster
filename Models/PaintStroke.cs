using System.Windows;
using System.Windows.Media;

namespace SprayMaster.Models
{
    public class PaintStroke
    {
        public List<Point> Points { get; set; } = new List<Point>();
        public Color Color { get; set; }
        public double Size { get; set; }
        public double Opacity { get; set; }
        public bool IsEraser { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
