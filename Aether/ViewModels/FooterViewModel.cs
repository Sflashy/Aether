using Aether.Messages;
using Aether.Models;
using Aether.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Threading;

namespace Aether.ViewModels;

public partial class FooterViewModel : BaseViewModel, IRecipient<DeviceUpdateMessage>
{
    [ObservableProperty]
    public partial Device Device { get; set; }
    public FooterViewModel(INotifierService notifierService, Dispatcher dispatcher) : base(notifierService, dispatcher)
    {
        WeakReferenceMessenger.Default.Register<DeviceUpdateMessage>(this);
    }

    public void Receive(DeviceUpdateMessage message)
    {
        _dispatcher.Invoke(() => Device = message.Value);
    }
}
