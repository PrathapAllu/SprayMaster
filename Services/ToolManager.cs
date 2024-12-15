using PropertyChanged;
using System.Windows.Media;
using static SprayMaster.Models.Tool;

[AddINotifyPropertyChangedInterface]
public class ToolManager
{
    public SolidColorBrush SelectedBrush { get; set; }
    private readonly List<Color> _colors;
    public IEnumerable<Color> Colors => _colors;
    public Color SelectedColor { get; set; }
    public double BrushSize { get; set; } = 10;
    public double Opacity { get; set; } = 1.0;
    public ToolType CurrentTool { get; set; } = ToolType.Spray;

    public ToolManager()
    {
        _colors = new List<Color>
        {
            System.Windows.Media.Colors.Black,
            System.Windows.Media.Colors.White,
            System.Windows.Media.Colors.Red,
            System.Windows.Media.Colors.Green,
            System.Windows.Media.Colors.Blue,
            System.Windows.Media.Colors.Yellow,
            System.Windows.Media.Colors.Purple,
            System.Windows.Media.Colors.Orange
        };
        SelectedColor = System.Windows.Media.Colors.Black;
        SelectedBrush = new SolidColorBrush(SelectedColor);
    }
}