using PropertyChanged;
using SprayMaster.Helpers;
using SprayMaster.Interface;
using SprayMaster.Models;
using SprayMaster.Services;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

[AddINotifyPropertyChangedInterface]
public class MainViewModel : IImageService
{
    public BitmapImage CurrentImage { get; set; }
    public double ZoomLevel { get; set; } = 1.0;
    public double ImageWidth { get; set; }
    public double ImageHeight { get; set; }
    public double ImageLeft { get; set; }
    public double ImageTop { get; set; }
    public string ImageName { get; set; } = "None";
    public string ImageFormat { get; set; }

    public ICommand LoadImageCommand { get; private set; }
    public ICommand SaveImageAsyncCommand { get; private set; }
    public ICommand SaveImageCommand { get; }
    public ICommand SavePaintDataAsyncCommand { get; private set; }

    public ToolManager ToolManager { get; set; }
    private DrawingManager drawingManager;

    public MainViewModel()
    {
        ToolManager = new ToolManager();
        LoadImageCommand = new RelayCommand<object>(_ => LoadAndScaleImage(null));
        SaveImageAsyncCommand = new RelayCommand<string>(async (path) => await SaveImageAsync(path, null));
        SavePaintDataAsyncCommand = new RelayCommand<string>(async (path) => await SavePaintDataAsync(path, null));
        SaveImageCommand = new RelayCommand<string>(async (path) => await SaveImage(path, null));

    }

    public Task SaveImageAsync(string path, BitmapSource image)
    {
        throw new NotImplementedException();
    }

    public Task SavePaintDataAsync(string path, PaintData data)
    {
        throw new NotImplementedException();
    }

    public Task SaveImage(string path, BitmapSource image)
    {
        throw new NotImplementedException();
    }

    //TODO: In CenterAndScaleImage remove Hardcoded canvas size and stretch based on uploaded image size
    public async Task<(BitmapImage Image, double Width, double Height)> LoadAndScaleImage(string path)
    {
        var image = await Task.Run(() => LoadImage(path));
        if (image?.Width > 0 && image?.Height > 0)
        {
            ImageWidth = image.Width;
            ImageHeight = image.Height;
            CurrentImage = image;
            CenterAndScaleImage(image, 800, 600);
            return (image, image.Width, image.Height);
        }
        return (null, 0, 0);
    }

    private BitmapImage LoadImage(string path)
    {
        try
        {
            if (string.IsNullOrEmpty(path))
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp"
                };
                if (dialog.ShowDialog() != true) return null;
                path = dialog.FileName;
                ImageName = System.IO.Path.GetFileName(path);
                ImageFormat = System.IO.Path.GetExtension(path).TrimStart('.').ToUpper();
            }

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(path);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.Freeze();
            return image;
        }
        catch
        {
            return null;
        }
    }

    public void CenterAndScaleImage(BitmapImage image, double canvasWidth, double canvasHeight)
    {
        if (image == null) return;

        double scaleX = canvasWidth / image.Width;
        double scaleY = canvasHeight / image.Height;
        ZoomLevel = Math.Min(scaleX, scaleY);
        ImageLeft = (canvasWidth - (image.Width * ZoomLevel)) / 2;
        ImageTop = (canvasHeight - (image.Height * ZoomLevel)) / 2;
        ImageWidth = image.Width;
        ImageHeight = image.Height;
    }

}