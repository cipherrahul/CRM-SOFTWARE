using CRM.DesktopClient.ViewModels;
using System.Windows;

namespace CRM.DesktopClient;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}