using SprayMaster.Models;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SprayMaster.Services
{
    public class PaintDataService
    {
        public List<PaintData> strokes { get; set; } = new();
        private readonly string sprayFileExtension = ".spray";

        private InkCanvas InkCanvas { get; set; }

        public PaintDataService() 
        {
            //TODO:Inject MainWindow via DI instead of accessing like this
            InkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
        }

        private Ellipse CreateEllipse(PaintData stroke)
        {
            return new Ellipse
            {
                Width = stroke.Size,
                Height = stroke.Size,
                Fill = new SolidColorBrush(stroke.Color)
            };
        }

        public void SaveSprayData(string imagePath)
        {
            var sprayDoc = new SprayDocument
            {
                ImagePath = imagePath,
                Strokes = strokes
            };

            string sprayPath = System.IO.Path.ChangeExtension(imagePath, sprayFileExtension);
            string json = JsonSerializer.Serialize(sprayDoc);
            File.WriteAllText(sprayPath, json);
        }

        public void LoadSprayData(string imagePath)
        {
            string sprayPath = System.IO.Path.ChangeExtension(imagePath, sprayFileExtension);
            if (!File.Exists(sprayPath)) return;

            string json = File.ReadAllText(sprayPath);
            var sprayDoc = JsonSerializer.Deserialize<SprayDocument>(json);

            InkCanvas.Children.Clear();
            strokes.Clear();

            foreach (var stroke in sprayDoc.Strokes)
            {
                strokes.Add(stroke);
                var ellipse = CreateEllipse(stroke);
                InkCanvas.SetLeft(ellipse, stroke.X - stroke.Size / 2);
                InkCanvas.SetTop(ellipse, stroke.Y - stroke.Size / 2);
                InkCanvas.Children.Add(ellipse);
            }
        }
    }
}
