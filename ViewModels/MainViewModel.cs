using Microsoft.Win32;
using PropertyChanged;
using SprayMaster;
using SprayMaster.Helpers;
using SprayMaster.Models;
using SprayMaster.Services;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;

[AddINotifyPropertyChangedInterface]
public class MainViewModel
{
    public ICommand LoadImageCommand { get; private set; }
    public ICommand SaveImageAsCommand { get; private set; }
    public ICommand SaveImageCommand { get; private set; }
    public ICommand ClearAllCommand { get; private set; }
    public ICommand SaveStrokeDataCommand { get; set; }

    public ToolManager toolManager { get; set; }
    public ImageService imageService { get; set; }

    public MainViewModel()
    {
        toolManager = new ToolManager();
        imageService = new ImageService();
        LoadImageCommand = new RelayCommand(imageService.LoadImage);
        SaveImageAsCommand = new RelayCommand(imageService.SaveAs);
        SaveImageCommand = new RelayCommand(imageService.Save);
        ClearAllCommand = new RelayCommand(imageService.ClearAll);
        SaveStrokeDataCommand = new RelayCommand(SaveStrokesData);

    }

    public void SaveStrokesData()
    {
    }

    public void LoadStrokesData()
    {
    }
}