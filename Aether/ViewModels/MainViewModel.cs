using Aether.Messages;
using Aether.Models;
using Aether.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using static Aether.Models.ConsoleOutput;


namespace Aether.ViewModels;

public partial class MainViewModel : BaseViewModel, IRecipient<ConsoleMessage>
{
    [ObservableProperty]
    public partial ObservableCollection<ConsoleOutput> ConsoleOutputs { get; set; }

    public MainViewModel(INotifierService notifier, Dispatcher dispatcher) : base(notifier, dispatcher)
    {
        Initialize();
    }

    private void Initialize()
    {
        ConsoleOutputs = new ObservableCollection<ConsoleOutput>();
        WeakReferenceMessenger.Default.Register<ConsoleMessage>(this);
        _notifier.NotifyConsole("Frida APK Injector v1.0", OutputType.Debug);
    }


    public void Receive(ConsoleMessage message)
    {
        if (message.Value == null) return;

        _dispatcher.Invoke(() => ConsoleOutputs.Add(message.Value));
    }

    #region WindowControls
    [RelayCommand]
    private void MinimizeWindow(Window window) => window.WindowState = WindowState.Minimized;

    [RelayCommand]
    private void CloseWindow(Window window) => Application.Current.Shutdown();

    [RelayCommand]
    private void DragWindow(Window window) => window.DragMove();

    [RelayCommand]
    private void MaximizeWindow(Window window) => window.WindowState = window.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
    #endregion
}
