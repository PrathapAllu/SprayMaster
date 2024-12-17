using Microsoft.Win32;
using PropertyChanged;
using SprayMaster.Interfaces;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

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
                    ImageName = System.IO.Path.GetFileName(openFileDialog.FileName);
                    ImageFormat = System.IO.Path.GetExtension(openFileDialog.FileName).TrimStart('.');

                    inkCanvas?.Children.Clear();
                    inkCanvas?.Children.Add(CurrentImage);
                    LoadAssociatedStrokes();
                    LoadSprayData();
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

        private void LoadSprayData()
        {
            try
            {
                var sprayPath = GetSprayPath();
                var inkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
                if (inkCanvas == null || !File.Exists(sprayPath)) return;

                var sprayXml = XDocument.Load(sprayPath);
                foreach (var ellipseElement in sprayXml.Root.Elements("Ellipse"))
                {
                    var ellipse = new Ellipse
                    {
                        Width = double.Parse(ellipseElement.Attribute("Width").Value),
                        Height = double.Parse(ellipseElement.Attribute("Height").Value),
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ellipseElement.Attribute("Fill").Value)),
                        Opacity = double.Parse(ellipseElement.Attribute("Opacity").Value)
                    };

                    InkCanvas.SetLeft(ellipse, double.Parse(ellipseElement.Attribute("Left").Value));
                    InkCanvas.SetTop(ellipse, double.Parse(ellipseElement.Attribute("Top").Value));
                    inkCanvas.Children.Add(ellipse);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading spray data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(lastImagePath))
            {
                SaveAs();
                return;
            }
            SaveImageAndStrokes(lastImagePath, false);
            MessageBox.Show("Edit Saved Successfully");
        }

        private void SaveImageAndStrokes(string imagePath, bool renderSprayToImage)
        {
            try
            {
                var inkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
                if (inkCanvas == null) return;

                using (var fs = new FileStream(GetStrokesPath(), FileMode.Create))
                {
                    inkCanvas.Strokes.Save(fs);
                }

                if (renderSprayToImage)
                {
                    RenderTargetBitmap rtb = new RenderTargetBitmap(
                        (int)inkCanvas.ActualWidth,
                        (int)inkCanvas.ActualHeight,
                        96, 96,
                        PixelFormats.Default);
                    rtb.Render(inkCanvas);

                    BitmapEncoder encoder = System.IO.Path.GetExtension(imagePath).ToLower() == ".png"
                        ? new PngBitmapEncoder()
                        : new JpegBitmapEncoder();

                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    using (var fs = new FileStream(imagePath, FileMode.Create))
                    {
                        encoder.Save(fs);
                    }
                }
                else
                {
                    SaveSprayData(inkCanvas);

                    if (CurrentImage?.Source is BitmapSource bitmapSource)
                    {
                        BitmapEncoder encoder = System.IO.Path.GetExtension(imagePath).ToLower() == ".png"
                            ? new PngBitmapEncoder()
                            : new JpegBitmapEncoder();

                        encoder.Frames.Add(BitmapFrame.Create((BitmapSource)CurrentImage.Source));
                        using (var fs = new FileStream(imagePath, FileMode.Create))
                        {
                            encoder.Save(fs);
                        }
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

        private void SaveSprayData(InkCanvas inkCanvas)
        {
            try
            {
                var sprayElements = inkCanvas.Children
                    .OfType<Ellipse>()
                    .ToList();

                var sprayXml = new XDocument(
                    new XElement("SprayData",
                        sprayElements.Select(ellipse =>
                            new XElement("Ellipse",
                                new XAttribute("Width", ellipse.Width),
                                new XAttribute("Height", ellipse.Height),
                                new XAttribute("Fill", ((SolidColorBrush)ellipse.Fill).Color.ToString()),
                                new XAttribute("Opacity", ellipse.Opacity),
                                new XAttribute("Left", InkCanvas.GetLeft(ellipse)),
                                new XAttribute("Top", InkCanvas.GetTop(ellipse))
                            )
                        )
                    )
                );

                sprayXml.Save(GetSprayPath());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving spray data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    SaveImageAndStrokes(saveFileDialog.FileName, true);
                    MessageBox.Show("Edit Saved Successfully");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in save dialog: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetStrokesPath()
        {
            return System.IO.Path.ChangeExtension(lastImagePath, ".isf");
        }

        private string GetSprayPath()
        {
            return System.IO.Path.ChangeExtension(lastImagePath, ".spray");
        }

        public void ClearAll()
        {
            var inkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
            if (inkCanvas != null)
            {
                inkCanvas.Strokes.Clear();
                inkCanvas.Children.Clear();
                if (CurrentImage != null)
                {
                    inkCanvas.Children.Add(CurrentImage);
                }
            }
        }
    }
}