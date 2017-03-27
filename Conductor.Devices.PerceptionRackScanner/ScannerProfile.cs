using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Conductor.Devices.PerceptionRackScanner
{
    public class ScannerProfile
    {

        private ScannerProfile()
        {
        }


        public ScannerProfile(string ResultsDirectory = @"C:\fluidX Output")
        {

            this.ScannerName = "Perception";
            this.ThrowExceptionOnError = true;
            this.ExeSearchPath = new List<string>();
            this.ExeSearchPath.Add(@"C:\Program Files (x86)\fluidX\Intellicode\Intellicode");
            this.ExeSearchPath.Add(@"C:\Program Files\fluidX\Intellicode\Intellicode");
            this.ExeName = "IntelliCodel.exe";
            this.ProfileName = "96 2.xtprof";
            this.InstrumentName = "Perception";
            this.ProcessName = "IntelliCodel";
            this.Port = 8001;
            this.FailedScanRetryCount = 0;
            this.ResultsDirectory = ResultsDirectory;
           // this.Conversion = ScannerProfile.FlipType.Flip96;
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
        public  FlipType Conversion { get; set; }


        
        public enum FlipType { None, Flip96 }




        public string ExeFolder
        {
            get
            {
                foreach (string folder in this.ExeSearchPath)
                    if (Directory.Exists(folder))
                        return folder;
                return null;
            }
        }

        public string FullExePath
        {
            get
            {
                string directory = this.ExeFolder;
                if (directory == null) return null;
                string fullPath = Path.Combine(directory, ExeName);
                if (File.Exists(fullPath)) return fullPath;
                return null;
            }
        }

        public string ProfileName { get; set; }
    }
}
