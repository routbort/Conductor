using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conductor.Devices.RackScanner
{
    public class RackScanEventLogEntry:EventArgs
    {

        public RackScanEventLogEntry (string Message)
        {
            this.Message = Message;
            this.When = DateTime.Now;
        }
        public string Message { get; private set; }
        public DateTime When { get; private set; }

    }
}
