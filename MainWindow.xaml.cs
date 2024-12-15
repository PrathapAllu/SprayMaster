using System.Windows;
using System.Windows.Input;

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

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            DataContext = viewModel;

            //This enables user to draw on empty page too
            Loaded += (s, e) => {
                viewModel.drawingManager.InitializeDrawingLayer(canvasGrid.ActualWidth, canvasGrid.ActualHeight);
                drawingLayer.Source = viewModel.drawingManager.GetDrawingLayer();
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

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Cursor = Cursors.Pen;
                isDrawing = true;
                lastPoint = e.GetPosition(drawingLayer);
                viewModel.drawingManager.StartDrawing(lastPoint);
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                var currentPoint = e.GetPosition(drawingLayer);
                viewModel.drawingManager.Draw(currentPoint,
                    viewModel.toolManager.SelectedColor,
                    viewModel.toolManager.Opacity,
                    viewModel.toolManager.BrushSize,
                    viewModel.toolManager.CurrentTool);
            }
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.Arrow;
            if (e.LeftButton == MouseButtonState.Released)
            {
                isDrawing = false;
                viewModel.drawingManager.StopDrawing();
            }
        }
    }
}