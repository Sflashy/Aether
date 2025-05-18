using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aether.Messages;

public class FridaDownloadStatusMessage : ValueChangedMessage<double>
{
    public FridaDownloadStatusMessage(double value) : base(value)
    {
    }
}
