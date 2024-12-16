using Microsoft.Win32;
using PropertyChanged;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
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
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
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

                    LoadAssociatedStrokes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadAssociatedStrokes()
        {
            var strokesPath = GetStrokesPath();
            var inkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
            if (inkCanvas == null || !File.Exists(strokesPath)) return;

            try
            {
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

        public void SaveAs()
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
        }

        private void SaveImageAndStrokes(string imagePath)
        {
            var inkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
            if (inkCanvas == null) return;

            try
            {
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
            if (inkCanvas != null) inkCanvas.Strokes.Clear();
        }
    }
}
