using Aether.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;

namespace Aether.Controls;

public partial class FridaScriptControl : UserControl
{
    public FridaViewModel ViewModel { get; set; }
    public FridaScriptControl()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<FridaViewModel>();
        DataContext = ViewModel;
    }
}
