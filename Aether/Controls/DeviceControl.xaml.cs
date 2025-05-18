using Aether.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;

namespace Aether.Controls;

public partial class DeviceControl : UserControl
{
    public DeviceViewModel ViewModel { get; set; }
    public DeviceControl()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<DeviceViewModel>();
        DataContext = ViewModel;
    }
}
