using PropertyChanged;
using SprayMaster.Models;
using SprayMaster.Services;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

[AddINotifyPropertyChangedInterface]
public class DrawingManager : IDrawingManager
{
    public WriteableBitmap drawingLayer;
    private Random _random = new Random();
    private bool _isDrawing;
    private Point _lastPoint;
    private PaintData _paintData;
    private PaintStroke _currentStroke;

    public DrawingManager()
    {
        _paintData = new PaintData();
    }

    //private void InitializeDrawingLayer()
    //{
    //    InitializeDrawingLayer(viewModel.ImageWidth, viewModel.ImageHeight);
    //    drawingLayer.Source = GetDrawingLayer();
    //}

    public void InitializeDrawingLayer(double width, double height)
    {
        drawingLayer = new WriteableBitmap(
            (int)width,
            (int)height,
            96,
            96,
            PixelFormats.Pbgra32,
            null);
        _paintData.CanvasSize = new Size(width, height);
    }

    public void StartDrawing(Point point)
    {
        _isDrawing = true;
        _lastPoint = point;
        _currentStroke = new PaintStroke
        {
            Points = new List<Point> { point },
            Timestamp = DateTime.Now
        };
    }

    public void StopDrawing()
    {
        if (_currentStroke != null)
        {
            _paintData.Strokes.Add(_currentStroke);
            _currentStroke = null;
        }
        _isDrawing = false;
    }

    public void Draw(Point currentPoint, Color color, double density, double size, Tool.ToolType toolType)
    {
        if (!_isDrawing || drawingLayer == null) return;

        if (_currentStroke != null)
        {
            _currentStroke.Points.Add(currentPoint);
            _currentStroke.Color = color;
            _currentStroke.Size = size;
            _currentStroke.Opacity = density;
            _currentStroke.IsEraser = toolType == Tool.ToolType.Erase;
        }

        drawingLayer.Lock();
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

                    if (x >= 0 && x < drawingLayer.PixelWidth &&
                        y >= 0 && y < drawingLayer.PixelHeight)
                    {
                        if (toolType == Tool.ToolType.Erase)
                        {
                            WritePixel(x, y, Colors.Transparent);
                        }
                        else
                        {
                            Color adjustedColor = Color.FromArgb(
                                (byte)(255 * density),
                                color.R,
                                color.G,
                                color.B);
                            WritePixel(x, y, adjustedColor);
                        }
                    }
                }
            }
        }
        finally
        {
            drawingLayer.Unlock();
        }
        _lastPoint = currentPoint;
    }

    private void WritePixel(int x, int y, Color color)
    {
        try
        {
            byte[] ColorData = { color.B, color.G, color.R, color.A };
            drawingLayer.WritePixels(new Int32Rect(x, y, 1, 1), ColorData, 4, 0);
        }
        catch (Exception)
        {
            // Handle pixel writing errors
        }
    }

    public WriteableBitmap GetDrawingLayer() => drawingLayer;

    public PaintData GetPaintData() => _paintData;

    public void LoadPaintData(PaintData data)
    {
        _paintData = data ?? new PaintData();
        RedrawStrokes();
    }

    private void RedrawStrokes()
    {
        if (drawingLayer == null || _paintData?.Strokes == null) return;

        foreach (var stroke in _paintData.Strokes)
        {
            for (int i = 1; i < stroke.Points.Count; i++)
            {
                Draw(stroke.Points[i],
                    stroke.Color,
                    stroke.Opacity,
                    stroke.Size,
                    stroke.IsEraser ? Tool.ToolType.Erase : Tool.ToolType.Spray);
            }
        }
    }
}