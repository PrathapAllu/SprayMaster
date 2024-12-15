using System.Windows.Ink;
using System.Windows.Media;

namespace SprayMaster.Models
{
    public class PaintData
    {
        public StrokeCollection Strokes { get; set; }
        public double BrushSize { get; set; }
        public double EraserSize { get; set; }
        public Color SelectedColor { get; set; }
        public string ImagePath { get; set; }
        public bool EraserMode { get; set; }
    }

}
