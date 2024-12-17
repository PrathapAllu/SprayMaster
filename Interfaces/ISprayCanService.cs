using SprayMaster.Helpers;
using System.Windows.Controls;

namespace SprayMaster.Interfaces
{
    public interface ISprayCanService
    {
        double EraserSize { get; set; }
        bool isSprayCanActive { get; set; }
        void Initialize(InkCanvas canvas);
        void OnCanvasMouseEvent(object sender, CanvasMouseEventArgs e);
        void StartSpraying();
        void StopSpraying();
    }
}
