using Microsoft.Win32;
using PropertyChanged;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SprayMaster.Services
{
    [AddINotifyPropertyChangedInterface]
    public class ImageService : IImageService
    {
        public double ImageWidth { get; set; }
        public double ImageHeight { get; set; }
        public string ImageName { get; set; } = "None";
        public string ImageFormat { get; set; }
        public Image CurrentImage { get; set; }


        public void LoadImage()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All files (*.*)|*.*"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var img = new Image();
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDialog.FileName, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    img.Source = bitmap;
                    CurrentImage = img;

                    ImageWidth = bitmap.Width;
                    ImageHeight = bitmap.Height;
                    ImageName = Path.GetFileName(openFileDialog.FileName);
                    ImageFormat = Path.GetExtension(openFileDialog.FileName).TrimStart('.');
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SaveAs()
        {
            if (CurrentImage == null) return;

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JPEG files (*.jpg)|*.jpg|PNG files (*.png)|*.png|Bitmap files (*.bmp)|*.bmp|All files (*.*)|*.*",
                DefaultExt = ".jpg"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var inkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
                    if (inkCanvas == null) return;

                    var renderBitmap = new RenderTargetBitmap(
                        (int)inkCanvas.ActualWidth,
                        (int)inkCanvas.ActualHeight,
                        96, 96, PixelFormats.Pbgra32);

                    renderBitmap.Render(inkCanvas);

                    BitmapEncoder encoder = saveFileDialog.FileName.EndsWith(".png") ?
                        new PngBitmapEncoder() : new JpegBitmapEncoder();

                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    using (var fs = File.Create(saveFileDialog.FileName))
                    {
                        encoder.Save(fs);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving image: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void Save()
        {
            if (CurrentImage?.Source is not BitmapImage bitmapImage || string.IsNullOrEmpty(bitmapImage.UriSource?.LocalPath)) return;

            try
            {
                var inkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
                if (inkCanvas == null) return;

                var renderBitmap = new RenderTargetBitmap(
                    (int)inkCanvas.ActualWidth,
                    (int)inkCanvas.ActualHeight,
                    96, 96, PixelFormats.Pbgra32);
                renderBitmap.Render(inkCanvas);

                BitmapEncoder encoder = bitmapImage.UriSource.LocalPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ?
                    new PngBitmapEncoder() : new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                using (var fs = File.Create(bitmapImage.UriSource.LocalPath))
                {
                    encoder.Save(fs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ClearAll()
        {
            var inkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
            inkCanvas.Strokes.Clear();
        }
    }
}
