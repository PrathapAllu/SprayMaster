using Microsoft.Win32;
using PropertyChanged;
using SprayMaster.Interfaces;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
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
        private string lastImagePath;

        public void LoadImage()
        {
            try
            {
                var inkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All files (*.*)|*.*"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDialog.FileName, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    var img = new Image { Source = bitmap };
                    CurrentImage = img;
                    lastImagePath = openFileDialog.FileName;

                    ImageWidth = bitmap.Width;
                    ImageHeight = bitmap.Height;
                    ImageName = Path.GetFileName(openFileDialog.FileName);
                    ImageFormat = Path.GetExtension(openFileDialog.FileName).TrimStart('.');

                    inkCanvas?.Children.Clear();
                    inkCanvas?.Children.Add(CurrentImage);
                    LoadAssociatedStrokes();
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show($"File access error: {ex.Message}", "IO Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAssociatedStrokes()
        {
            try
            {
                var strokesPath = GetStrokesPath();
                var inkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
                if (inkCanvas == null || !File.Exists(strokesPath)) return;

                using (var fs = new FileStream(strokesPath, FileMode.Open))
                {
                    inkCanvas.Strokes = new StrokeCollection(fs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading strokes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Save()
        {

            if (string.IsNullOrEmpty(lastImagePath))
            {
                SaveAs();
                return;
            }
            SaveImageAndStrokes(lastImagePath);
            MessageBox.Show("Edit Saved Successfully");
        }

        private void SaveImageAndStrokes(string imagePath)
        {
            try
            {
                var inkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
                if (inkCanvas == null) return;

                using (var fs = new FileStream(GetStrokesPath(), FileMode.Create))
                {
                    inkCanvas.Strokes.Save(fs);
                }

                if (CurrentImage?.Source is BitmapSource bitmapSource)
                {
                    BitmapEncoder encoder = Path.GetExtension(imagePath).ToLower() == ".png"
                        ? new PngBitmapEncoder()
                        : new JpegBitmapEncoder();

                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)CurrentImage.Source));
                    using (var fs = new FileStream(imagePath, FileMode.Create))
                    {
                        encoder.Save(fs);
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show($"File access error: {ex.Message}", "IO Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SaveAs()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "JPEG files (*.jpg)|*.jpg|PNG files (*.png)|*.png|Bitmap files (*.bmp)|*.bmp|All files (*.*)|*.*",
                    DefaultExt = ".jpg"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    lastImagePath = saveFileDialog.FileName;
                    SaveImageAndStrokes(saveFileDialog.FileName);
                    MessageBox.Show("Edit Saved Successfully");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in save dialog: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveAsImageAndStrokes(string imagePath)
        {
            try
            {
                var inkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
                if (inkCanvas == null) return;

                using (var fs = new FileStream(GetStrokesPath(), FileMode.Create))
                {
                    inkCanvas.Strokes.Save(fs);
                }

                RenderTargetBitmap rtb = new RenderTargetBitmap(
                    (int)inkCanvas.ActualWidth,
                    (int)inkCanvas.ActualHeight,
                    96, 96,
                    PixelFormats.Default);
                rtb.Render(inkCanvas);

                BitmapEncoder encoder = Path.GetExtension(imagePath).ToLower() == ".png"
                    ? new PngBitmapEncoder()
                    : new JpegBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(rtb));
                using (var fs = new FileStream(imagePath, FileMode.Create))
                {
                    encoder.Save(fs);
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show($"File access error: {ex.Message}", "IO Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetStrokesPath()
        {
            return Path.ChangeExtension(lastImagePath, ".isf");
        }

        public void ClearAll()
        {
            var inkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
            if (inkCanvas != null)
            {
                inkCanvas.Strokes.Clear();
                inkCanvas.Children.Clear();
            }
        }
    }
}