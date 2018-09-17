using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conductor.Devices.RackScanner
{
    public interface IRackScanner
    {

        event EventHandler<RackScanResult> RackScanned;
        event EventHandler<RackScanEventLogEntry> RackScannerLogEvent;
        void ScanAsync();
        RackScanResult Scan();

    }
}
