using System;
using System.Collections.Generic;
using System.Text;

namespace ServerData
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ServerOnlyAttribute : Attribute
    {
    }
}
