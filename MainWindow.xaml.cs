using SprayMaster.Helpers;
using SprayMaster.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SprayMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel viewModel;
        public event EventHandler<CanvasMouseEventArgs> CanvasMouseEvent;

        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = mainViewModel;
        }

        #region control bar and mouse moments
        private void InkCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CanvasMouseEvent?.Invoke(this, new CanvasMouseEventArgs
            {
                Position = e.GetPosition(canvasPanel),
                IsPressed = true
            });
        }

        private void InkCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                CanvasMouseEvent?.Invoke(this, new CanvasMouseEventArgs
                {
                    Position = e.GetPosition(canvasPanel),
                    IsPressed = true
                });
            }
        }

        private void InkCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CanvasMouseEvent?.Invoke(this, new CanvasMouseEventArgs
            {
                Position = e.GetPosition(canvasPanel),
                IsPressed = false
            });
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
        #endregion

    }
}