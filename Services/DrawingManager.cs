using PropertyChanged;
using SprayMaster.Models;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

[AddINotifyPropertyChangedInterface]
public class DrawingManager
{
    private WriteableBitmap _drawingLayer;
    private Random _random = new Random();
    private bool _isDrawing;
    private Point _lastPoint;

    public void InitializeDrawingLayer(double width, double height)
    {
        _drawingLayer = new WriteableBitmap(
            (int)width,
            (int)height,
            96,
            96,
            PixelFormats.Pbgra32,
            null);
    }

    public void StartDrawing(Point point)
    {
        _isDrawing = true;
        _lastPoint = point;
    }

    public void StopDrawing()
    {
        _isDrawing = false;
    }

    public void Draw(Point currentPoint, Color color, double density, double size, Tool.ToolType toolType)
    {
        if (!_isDrawing || _drawingLayer == null) return;

        _drawingLayer.Lock();

        try
        {
            int particleCount = (int)(density * size);
            unsafe
            {
                for (int i = 0; i < particleCount; i++)
                {
                    double angle = _random.NextDouble() * Math.PI * 2;
                    double distance = _random.NextDouble() * size;

                    int x = (int)(currentPoint.X + Math.Cos(angle) * distance);
                    int y = (int)(currentPoint.Y + Math.Sin(angle) * distance);

                    if (x >= 0 && x < _drawingLayer.PixelWidth &&
                        y >= 0 && y < _drawingLayer.PixelHeight)
                    {
                        if (toolType == Tool.ToolType.Erase)
                        {
                            WritePixel(x, y, Colors.Transparent);
                        }
                        else
                        {
                            WritePixel(x, y, color);
                        }
                    }
                }
            }
        }
        finally
        {
            _drawingLayer.Unlock();
        }

        _lastPoint = currentPoint;
    }

    private void WritePixel(int x, int y, Color color)
    {
        try
        {
            int column = x;
            int row = y;

            byte[] ColorData = { color.B, color.G, color.R, color.A };
            int stride = _drawingLayer.PixelWidth * 4;
            int offset = row * stride + column * 4;

            _drawingLayer.WritePixels(new Int32Rect(column, row, 1, 1), ColorData, 4, 0);
        }
        catch (Exception)
        {
            // Handle pixel writing errors
        }
    }

    public WriteableBitmap GetDrawingLayer() => _drawingLayer;
}