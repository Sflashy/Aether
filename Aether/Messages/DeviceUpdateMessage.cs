using Aether.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Aether.Messages;

public class DeviceUpdateMessage : ValueChangedMessage<Device>
{
    public DeviceUpdateMessage(Device value) : base(value)
    {
    }
}
