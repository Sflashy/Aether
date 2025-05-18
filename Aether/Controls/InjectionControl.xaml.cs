using Aether.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;

namespace Aether.Controls;

public partial class InjectionControl : UserControl
{
    public InjectionViewModel ViewModel { get; set; }
    public InjectionControl()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<InjectionViewModel>();
        DataContext = ViewModel;
    }
}
