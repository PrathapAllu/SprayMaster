using System.Windows.Controls;

namespace SprayMaster.Services
{
    public interface IImageService
    {
        void LoadImage();
        void SaveAs();
        void Save();
        void ClearAll();
    }
}