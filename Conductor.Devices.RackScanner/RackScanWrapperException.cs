using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conductor.Devices.RackScanner
{
    public class RackScanWrapperException : Exception
    {
        public RackScanWrapperException()
        {
        }

        public RackScanWrapperException(string message)
            : base(message)
        {
        }

        public RackScanWrapperException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
