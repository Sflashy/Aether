using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APKTool.Messages;

public class DownloadStatusMessage : ValueChangedMessage<double>
{
    public DownloadStatusMessage(double value) : base(value)
    {
    }
}
