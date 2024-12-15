using System.Windows;

namespace SprayMaster.Models
{
    public class PaintData
    {
        public List<PaintStroke> Strokes { get; set; } = new List<PaintStroke>();
        public string OriginalImagePath { get; set; }
        public Size CanvasSize { get; set; }
    }
}