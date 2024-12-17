using Microsoft.Extensions.DependencyInjection;
using SprayMaster.Services;
using SprayMaster.ViewModels;
using System.Windows;

namespace SprayMaster
{
    public partial class App : Application
    {
        private ServiceProvider serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();

            try
            {
                var window = serviceProvider.GetRequiredService<MainWindow>();
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Startup error: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Register services
            services.AddSingleton<ToolManager>();
            services.AddSingleton<ImageService>();
            services.AddSingleton<SprayCanService>();

            // Create MainViewModel
            services.AddSingleton<MainViewModel>(provider => {
                var toolManager = provider.GetRequiredService<ToolManager>();
                var imageService = provider.GetRequiredService<ImageService>();
                var sprayService = provider.GetRequiredService<SprayCanService>();
                return new MainViewModel(toolManager, imageService, sprayService);
            });

            // Register MainWindow last
            services.AddSingleton<MainWindow>(provider => {
                var viewModel = provider.GetRequiredService<MainViewModel>();
                var sprayService = provider.GetRequiredService<SprayCanService>();
                var window = new MainWindow(viewModel);

                // Initialize services after window creation
                viewModel.Initialize(window.canvasPanel);
                sprayService.Initialize(window.canvasPanel);
                window.CanvasMouseEvent += sprayService.OnCanvasMouseEvent;

                return window;
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}