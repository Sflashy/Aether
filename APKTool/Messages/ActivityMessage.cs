using APKTool.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace APKTool.Messages;

public class ActivityMessage : ValueChangedMessage<Activity>
{
    public ActivityMessage(Activity value) : base(value)
    {
    }
}
