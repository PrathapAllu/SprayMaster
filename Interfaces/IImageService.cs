using System.Windows.Controls;

namespace SprayMaster.Interfaces
{
    public interface IImageService
    {
        void LoadImage();
        void SaveAs();
        void Save();
        void ClearAll();
    }
}