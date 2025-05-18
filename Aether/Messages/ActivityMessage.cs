using Aether.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Aether.Messages;

public class ActivityMessage : ValueChangedMessage<Activity>
{
    public ActivityMessage(Activity value) : base(value)
    {
    }
}
