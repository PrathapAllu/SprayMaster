using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace SprayMaster.Interface
{
    public interface ISprayPaintService
    {
        void SprayAt(Point point, Color color, double density);
        void Erase(Point point, double size, Canvas canvas);
    }
}
