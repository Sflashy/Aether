using Aether.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Aether.Messages;

public class ConsoleMessage : ValueChangedMessage<ConsoleOutput>
{
    public ConsoleMessage(ConsoleOutput value) : base(value)
    {
    }
}
