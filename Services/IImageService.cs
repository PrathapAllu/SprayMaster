using SprayMaster.Models;
using System.Windows.Media.Imaging;

namespace SprayMaster.Services
{
    public interface IImageService
    {
        public Task<(BitmapImage Image, double Width, double Height)> LoadAndScaleImage(string path);
        Task SaveImage(string path, BitmapSource image);
        Task SaveImageAsync(string path, BitmapSource image);
        Task SavePaintDataAsync(string path, PaintData data);
        void CenterAndScaleImage(BitmapImage image, double canvasWidth, double canvasHeight);
    }
}
