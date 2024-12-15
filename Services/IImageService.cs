using System.Windows.Controls;

namespace SprayMaster.Services
{
    public interface IImageService
    {
        double ImageWidth { get; set; }
        double ImageHeight { get; set; }
        string ImageName { get; set; }
        string ImageFormat { get; set; }
        Image CurrentImage { get; set; }

        void LoadImage();
        void SaveAs();
        void Save();
        void ClearAll();
    }
}