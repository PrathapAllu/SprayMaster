using PropertyChanged;
using SprayMaster;
using SprayMaster.Helpers;
using SprayMaster.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static SprayMaster.Models.Tool;

[AddINotifyPropertyChangedInterface]
public class MainViewModel
{
    public ICommand LoadImageCommand { get; private set; }
    public ICommand SaveImageAsCommand { get; private set; }
    public ICommand SaveImageCommand { get; private set; }
    public ICommand ClearAllCommand { get; private set; }
    public ICommand SaveStrokeDataCommand { get; set; }
    public ICommand ActivateSprayCommand { get; set; }

    public ToolManager toolManager { get; set; }
    public ImageService imageService { get; set; }
    public SprayCanService sprayCanService { get; set; }

    public MainViewModel()
    {
        var inkCanvas = (Application.Current.MainWindow as MainWindow)?.canvasPanel;
        toolManager = new ToolManager();
        imageService = new ImageService();
        sprayCanService = new SprayCanService(inkCanvas, toolManager.BrushSize, toolManager.DrawingAttributes);
        LoadImageCommand = new RelayCommand(imageService.LoadImage);
        SaveImageAsCommand = new RelayCommand(imageService.SaveAs);
        SaveImageCommand = new RelayCommand(imageService.Save);
        ClearAllCommand = new RelayCommand(imageService.ClearAll);
        ActivateSprayCommand = new RelayCommand(() => {
            if (toolManager.CurrentTool == ToolType.Spray)
            {
                sprayCanService.StartSpraying();
                inkCanvas.EditingMode = InkCanvasEditingMode.None;
                toolManager.SprayCan();
            }
            else
            {
                sprayCanService.StopSpraying();
                inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            }
        });
        SaveStrokeDataCommand = new RelayCommand(SaveStrokesData);

    }

    public void SaveStrokesData()
    {
    }

    public void LoadStrokesData()
    {
    }
}