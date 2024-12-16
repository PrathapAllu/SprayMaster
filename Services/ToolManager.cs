using PropertyChanged;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using static SprayMaster.Models.Tool;

namespace SprayMaster.Services
{
    [AddINotifyPropertyChangedInterface]
    public class ToolManager
    {
        public DrawingAttributes DrawingAttributes { get; } = new();
        public Color SelectedColor
        {
            get => DrawingAttributes.Color;
            set => DrawingAttributes.Color = value;
        }
        public SolidColorBrush SelectedBrush { get; set; }
        public List<Color> Colors { get; set; }
        public double BrushSize
        {
            get => DrawingAttributes.Width;
            set => DrawingAttributes.Width = DrawingAttributes.Height = value;
        }
        public ToolType CurrentTool { get; set; }
        public bool isSprayCanActive = false;
        public bool isUseEraser = false;

        public ToolManager()
        {
            Colors = new List<Color>
        {
                System.Windows.Media.Colors.Black,
                System.Windows.Media.Colors.White,
                System.Windows.Media.Colors.Red,
                System.Windows.Media.Colors.Green,
                System.Windows.Media.Colors.Blue,
                System.Windows.Media.Colors.Yellow,
                System.Windows.Media.Colors.Purple,
                System.Windows.Media.Colors.Orange,
                System.Windows.Media.Colors.Pink,
                System.Windows.Media.Colors.Brown,
                System.Windows.Media.Colors.Gray,
                System.Windows.Media.Colors.Cyan,
                System.Windows.Media.Colors.Magenta,
                System.Windows.Media.Colors.Lime,
                System.Windows.Media.Colors.Teal
        };
            SelectedColor = System.Windows.Media.Colors.Red;
            SelectedBrush = new SolidColorBrush(SelectedColor);
        }

        public void SprayCan()
        {
            CurrentTool = ToolType.Spray;
            DrawingAttributes.IgnorePressure = true;
        }

        public void UseEraser(bool use)
        {
            var inkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
            var sliderValue = 2;
            isUseEraser = use;
            if (inkCanvas != null)
            {
                if (isUseEraser)
                {
                    inkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
                    inkCanvas.EraserShape = new EllipseStylusShape(2,2);
                }
                else
                {
                    inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                }
            }
        }
    }
}
