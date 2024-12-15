using System.Windows.Media;

namespace SprayMaster.Models
{
    public class PaintData
    {
        public double BrushSize { get; set; }
        public double EraserSize { get; set; }
        public Color SelectedColor { get; set; }
        public string ImagePath { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Size { get; set; }
        public Color Color { get; set; }
    }

}
