using APKTool.ViewModels;
using System.Windows;

namespace APKTool.Views;

public partial class MainView : Window
{
    public MainViewModel ViewModel { get; set; }
    public MainView(MainViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = ViewModel;
    }
}