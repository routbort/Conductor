using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Conductor.Devices.RackScanner
{
    public class ZiathScannerProfile
    {


        public ZiathScannerProfile()
        {

            this.ScannerName = "Ziath";
            this.ThrowExceptionOnError = true;
            this.ExeName = "server.exe";
            this.ProfileName = "1";
            this.InstrumentName = "Ziath";
            this.ProcessName = "server";
            this.Port = 8888;
        }

        public int FailedScanRetryCount { get; set; }
        public int Port { get; private set; }
        public string ProcessName { get; set; }
        public string ScannerName { get; set; }
        public bool ThrowExceptionOnError { get; set; }
        public List<string> ExeSearchPath { get; private set; }
        public string ExeName { get; set; }
        public string ResultsDirectory { get;private set; }
        public string InstrumentName { get; set; }
        public string ProfileName { get; set; }
    }
}
