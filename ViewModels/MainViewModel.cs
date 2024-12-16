using PropertyChanged;
using SprayMaster.Helpers;
using SprayMaster.Services;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static SprayMaster.Models.Tool;

namespace SprayMaster.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        private InkCanvas inkCanvas;
        public ToolManager toolManager { get; }
        public ImageService imageService { get; }
        public SprayCanService sprayCanService { get; }

        public ICommand LoadImageCommand { get; private set; }
        public ICommand SaveImageAsCommand { get; private set; }
        public ICommand SaveImageCommand { get; private set; }
        public ICommand ClearAllCommand { get; private set; }
        public ICommand ActivateSprayCommand { get; set; }
        public ICommand UseEraserCommand { get; set; }
        public ICommand UnavailableCommand { get; set; }

        public MainViewModel(ToolManager toolManager, ImageService imageService, SprayCanService sprayCanService)
        {
            this.toolManager = toolManager;
            this.imageService = imageService;
            this.sprayCanService = sprayCanService;
            InitializeCommands();
        }

        public void Initialize(InkCanvas canvas)
        {
            this.inkCanvas = canvas;
        }

        private void InitializeCommands()
        {
            LoadImageCommand = new RelayCommand(imageService.LoadImage);
            SaveImageAsCommand = new RelayCommand(imageService.SaveAs);
            SaveImageCommand = new RelayCommand(imageService.Save);
            ClearAllCommand = new RelayCommand(imageService.ClearAll);

            ActivateSprayCommand = new RelayCommand(() => {
                toolManager.isSprayCanActive = !toolManager.isSprayCanActive;
                if (toolManager.CurrentTool == ToolType.Spray && toolManager.isSprayCanActive)
                {
                    sprayCanService.StartSpraying();
                    inkCanvas.EditingMode = InkCanvasEditingMode.None;
                    toolManager.SprayCan();
                }
                else
                {
                    sprayCanService.StopSpraying();
                    inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                }
            });

            UseEraserCommand = new RelayCommand(() => {
                toolManager.isUseEraser = !toolManager.isUseEraser;
                toolManager.UseEraser(toolManager.isUseEraser);
            });

            UnavailableCommand = new RelayCommand(DefaultSystemsMessages);
        }

        public void SaveStrokesData()
        {
            var inkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
            if (inkCanvas == null || imageService.CurrentImage == null) return;

            try
            {
                var strokesPath = Path.ChangeExtension(imageService.CurrentImage.Name, ".isf");
                using (var fs = new FileStream(strokesPath, FileMode.Create))
                {
                    inkCanvas.Strokes.Save(fs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving strokes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DefaultSystemsMessages()
        {
            MessageBox.Show("This feature is currently unavailable.", "System Message");
        }
    }
}
