using APKTool.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace APKTool.Messages;

public class ConsoleMessage : ValueChangedMessage<ConsoleOutput>
{
    public ConsoleMessage(ConsoleOutput value) : base(value)
    {
    }
}
