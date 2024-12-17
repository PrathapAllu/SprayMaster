using SprayMaster.Services;
using SprayMaster.ViewModels;
using System.Windows;

namespace SprayMaster
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                var toolManager = new ToolManager();
                var imageService = new ImageService();
                var sprayService = new SprayCanService(toolManager);

                var mainViewModel = new MainViewModel(toolManager, imageService, sprayService);

                var window = new MainWindow();
                window.DataContext = mainViewModel;

                mainViewModel.Initialize(window.canvasPanel);
                sprayService.Initialize(window.canvasPanel);
                window.CanvasMouseEvent += sprayService.OnCanvasMouseEvent;

                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Startup error: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}