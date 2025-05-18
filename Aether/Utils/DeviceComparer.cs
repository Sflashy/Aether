using Aether.Models;

namespace Aether.Utils;

public class DeviceComparer : IEqualityComparer<Device>
{
    public bool Equals(Device x, Device y)
    {
        if (x == null || y == null)
            return false;

        return x.Id == y.Id;
    }

    public int GetHashCode(Device obj)
    {
        return obj.Id.GetHashCode();
    }
}
