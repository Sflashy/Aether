using APKTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APKTool.Utils;

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
