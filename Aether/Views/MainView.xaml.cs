using Aether.ViewModels;
using System.Windows;

namespace Aether.Views;

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