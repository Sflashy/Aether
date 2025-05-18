using Aether.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;

namespace Aether.Controls;

public partial class FridaGadgetControl : UserControl
{
    public FridaViewModel ViewModel { get; set; }
    public FridaGadgetControl()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<FridaViewModel>();
        DataContext = ViewModel;
    }
}
