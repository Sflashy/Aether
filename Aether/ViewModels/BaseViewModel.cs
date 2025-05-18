using Aether.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Threading;

namespace Aether.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    protected readonly INotifierService _notifier;
    protected readonly Dispatcher _dispatcher;
    
    protected BaseViewModel(INotifierService notifierService, Dispatcher dispatcher)
    {
        _notifier = notifierService;
        _dispatcher = dispatcher;
    }

}
