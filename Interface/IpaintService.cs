using System.Windows.Media;
using System.Windows;

namespace SprayMaster.Interface
{
    public interface IPaintService
    {
        void ApplySpray(Point position, double size, Color color, double opacity);
        void Erase(Point position, double size);
        void Undo();
        void Clear();
        ImageSource GetCompositeImage();
    }
}
