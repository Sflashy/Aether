using Aether.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Aether.Controls;

public partial class WorkspaceControl : UserControl
{
    public WorkspaceViewModel ViewModel { get; set; }
    public WorkspaceControl()
    {
        InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<WorkspaceViewModel>();
        DataContext = ViewModel;
    }

    private void CopyEvent(object sender, MouseButtonEventArgs e)
    {
        if (sender is Run run)
        {
            Clipboard.SetText(run.Text.ToLower());
        }
        
    }
}