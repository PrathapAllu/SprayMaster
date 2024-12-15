using PropertyChanged;
using SprayMaster.Helpers;
using SprayMaster.Services;
using System.Windows.Input;

[AddINotifyPropertyChangedInterface]
public class MainViewModel
{
    public ICommand LoadImageCommand { get; private set; }
    public ICommand SaveImageAsyncCommand { get; private set; }
    public ICommand SaveImageCommand { get; }
    public ICommand SavePaintDataAsyncCommand { get; private set; }

    public ToolManager toolManager { get; set; }
    public DrawingManager drawingManager;
    public ImageService imageService { get; set; }

    public MainViewModel()
    {
        toolManager = new ToolManager();
        imageService = new ImageService();
        drawingManager = new DrawingManager();
        LoadImageCommand = new RelayCommand<object>(async _ => await imageService.LoadAndScaleImage(null));
        SaveImageAsyncCommand = new RelayCommand<string>(async (path) => await imageService.SaveImageAsync(path, null));
        SavePaintDataAsyncCommand = new RelayCommand<string>(async (path) => await imageService.SavePaintDataAsync(path, null));
        SaveImageCommand = new RelayCommand<string>(async (path) => await imageService.SaveImage(path, null));
    }
}