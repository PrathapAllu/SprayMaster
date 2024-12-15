using SprayMaster.Models;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SprayMaster.Services
{
    public interface IDrawingManager
    {
        void InitializeDrawingLayer(double width, double height);
        void StartDrawing(Point point);
        void StopDrawing();
        void Draw(Point currentPoint, Color color, double density, double size, Tool.ToolType toolType);
        WriteableBitmap GetDrawingLayer();
        PaintData GetPaintData();
        void LoadPaintData(PaintData data);
    }
}

