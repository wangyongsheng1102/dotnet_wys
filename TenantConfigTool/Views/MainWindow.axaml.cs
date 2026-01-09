using Avalonia.Controls;
using TenantConfigTool.ViewModels;

namespace TenantConfigTool.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(MainViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.SetParentWindow(this);
    }
}
