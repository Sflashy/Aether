using Aether.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aether.Exceptions;

public class EnvironmentInitializeException : Exception
{
    public EnvironmentInitializeException(){}
    public EnvironmentInitializeException(string message) : base(message){}
}
