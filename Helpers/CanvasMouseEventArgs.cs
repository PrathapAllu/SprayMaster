using System.Windows;

namespace SprayMaster.Helpers
{
    public class CanvasMouseEventArgs : EventArgs
    {
        public Point Position { get; set; }
        public bool IsPressed { get; set; }
    }
}
