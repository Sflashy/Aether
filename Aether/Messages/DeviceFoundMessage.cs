using Aether.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Aether.Messages;

public class DeviceFoundMessage : ValueChangedMessage<Device>
{
    public DeviceFoundMessage(Device value) : base(value)
    {
    }
}
