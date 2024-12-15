using SprayMaster.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SprayMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isDrawing;
        private Point lastPoint;
        private readonly MainViewModel viewModel;
        private DrawingManager drawingManager;


        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            drawingManager = new DrawingManager();
            DataContext = viewModel;
            InitializeDrawingLayer();
        }

        private void InitializeDrawingLayer()
        {
            drawingManager.InitializeDrawingLayer(800, 600);
            drawingLayer.Source = drawingManager.GetDrawingLayer();
        }

        private void LoadPredefinedColors()
        {
            var colors = new List<SolidColorBrush>
    {
        new SolidColorBrush(Colors.Black),
        new SolidColorBrush(Colors.White),
        new SolidColorBrush(Colors.Red),
        new SolidColorBrush(Colors.Green),
        new SolidColorBrush(Colors.Blue),
        new SolidColorBrush(Colors.Yellow),
        new SolidColorBrush(Colors.Purple),
        new SolidColorBrush(Colors.Orange)
    };
        }

        private void panelControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void panelnlControlBar_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else this.WindowState = WindowState.Normal;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.Pen;
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                isDrawing = true;
                lastPoint = e.GetPosition(imageCanvas);
                drawingManager.StartDrawing(lastPoint);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                var currentPoint = e.GetPosition(imageCanvas);
                drawingManager.Draw(currentPoint,
                    viewModel.ToolManager.SelectedColor,
                    viewModel.ToolManager.BrushSize,
                    viewModel.ToolManager.Opacity,
                    viewModel.ToolManager.CurrentTool);
            }
        }


        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {

            Cursor = Cursors.Arrow;
            if (e.LeftButton == MouseButtonState.Released)
            {
                isDrawing = false;
                drawingManager.StopDrawing();
            }
        }
    }
}