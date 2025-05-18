using Aether.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;

namespace Aether.Controls;

public partial class FooterControl : UserControl
{
    public FooterViewModel ViewModel { get; set; }
    public FooterControl()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<FooterViewModel>();
        DataContext = ViewModel;
    }
}
