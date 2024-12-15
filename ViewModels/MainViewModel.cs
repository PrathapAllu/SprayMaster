using PropertyChanged;
using SprayMaster.Helpers;
using SprayMaster.Services;
using System.Windows.Input;

[AddINotifyPropertyChangedInterface]
public class MainViewModel
{
    public ICommand LoadImageCommand { get; private set; }
    public ICommand SaveImageAsCommand { get; private set; }
    public ICommand SaveImageCommand { get; private set; }

    public ToolManager toolManager { get; set; }
    public ImageService imageService { get; set; }

    public MainViewModel()
    {
        toolManager = new ToolManager();
        imageService = new ImageService();
        LoadImageCommand = new RelayCommand(imageService.LoadImage);
        SaveImageAsCommand = new RelayCommand(imageService.SaveAs);
        SaveImageCommand = new RelayCommand(imageService.Save);
    }
}