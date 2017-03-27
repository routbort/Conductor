using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Threading;

namespace Conductor.Devices.PerceptionRackScanner
{
    public class RackScanner
    {
        public delegate void RackScannedEventHandler(RackScanResult data);
        public event RackScannedEventHandler RackScanned;

        public delegate void RackScannerLogEventHandler(string message);
        public event RackScannerLogEventHandler RackScannerLogEvent;

        RackScanResult _result = null;
        Dictionary<string, string> _Flip96;
        MinimalisticTelnet.TelnetConnection2 _tc = null;

        public class RackScannerInitializationResult
        {
            public enum ResultType { OK, Failed };
            public ResultType InitializationResult { get; set; }
            public string ErrorMessage { get; set; }
        }

        public string ErrorMessage { get; private set; }

         RackScanner() { }

         void LogEvent(string message)
        {
            if (this.RackScannerLogEvent != null)
                RaiseEventOnUIThread(this.RackScannerLogEvent, new object[] { message });
        }

        public RackScanner(ScannerProfile Profile)
        {
            this.Profile = Profile;
            if (!Directory.Exists(this.Profile.ResultsDirectory))
            {
                GenerateError("Invalid profile results directory: " + this.Profile.ResultsDirectory);
                return;
            }
            FileSystemWatcher fsw = new FileSystemWatcher(this.Profile.ResultsDirectory);
            fsw.Created += new FileSystemEventHandler(fsw_Created);
            fsw.EnableRaisingEvents = true;
        }



        string Convert(ScannerProfile.FlipType flipType, string address)
        {

            if (_Flip96 == null)
            {
                _Flip96 = new Dictionary<string, string>();

                _Flip96["A"] = "H";
                _Flip96["B"] = "G";
                _Flip96["C"] = "F";
                _Flip96["D"] = "E";
                _Flip96["E"] = "D";
                _Flip96["F"] = "C";
                _Flip96["G"] = "B";
                _Flip96["H"] = "A";
                _Flip96["1"] = "12";
                _Flip96["2"] = "11";
                _Flip96["3"] = "10";
                _Flip96["4"] = "9";
                _Flip96["5"] = "8";
                _Flip96["6"] = "7";
                _Flip96["7"] = "6";
                _Flip96["8"] = "5";
                _Flip96["9"] = "4";
                _Flip96["10"] = "3";
                _Flip96["11"] = "2";
                _Flip96["12"] = "1";
            }


            if (flipType != ScannerProfile.FlipType.Flip96) throw new ApplicationException("Not supported");

            string convertedValue = _Flip96[address.Substring(0, 1)] + _Flip96[address.Substring(1)];
            return convertedValue;



        }

        bool _ScanInProgress = false;


        void fsw_Created(object sender, FileSystemEventArgs e)
        {
            //If we are not scanning actively, do not generate an event
            if (!_ScanInProgress)
                return;

            System.Diagnostics.Debug.WriteLine(e.FullPath);
            System.Diagnostics.Debug.WriteLine(this.Profile.Conversion.ToString());

            LogEvent("File found in output directory: " + e.Name);

   
            _ScanInProgress = false;

            RackScanResult result = new RackScanResult();
            int FIELD_COUNT = 2;

            string[] lines = null;
            int failure_count = 0;
            while (true)
            {
                try
                {
                    lines = File.ReadAllLines(e.FullPath);
                    break;
                }
                catch
                {
                    failure_count++;
                    System.Threading.Thread.Sleep(200);
                    if (failure_count > 20)
                        GenerateError("Could not get access to locked file " + e.FullPath);
                }
            }

            System.Diagnostics.Debug.WriteLine("Lines: " + lines.Length.ToString());

            foreach (string line in lines)
            {
                string[] items = line.Split(new char[] { ',' });
                if (items.Length != FIELD_COUNT)
                {
                    result.HasError = true;
                    result.ErrorDetail = "File failed parsing, expected " + FIELD_COUNT.ToString() + " fields, found " + items.Length.ToString();
                    RaiseRackScannedEvent(result);
                    return;
                }

                string address = items[0].Trim();
                string element_barcode = items[1].Trim();

                if (this.Profile.Conversion != ScannerProfile.FlipType.None)
                    address = Convert(this.Profile.Conversion, address);


                if (this.PadColumnAddress)
                {

                    string row_address = address.Substring(0, 1);
                    string col_address = address.Substring(1);
                    col_address = System.Convert.ToInt32(col_address).ToString("D2");
                    address = row_address + col_address;
                }


                //    string rack_barcode = items[3].Trim();
                ////    if (result.RackBarcode == null)
                //       result.RackBarcode = rack_barcode;
                ///   if (result.RackBarcode != rack_barcode)
                //  {
                //      result.HasError = true;
                //    result.ErrorDetail = "File failed validity check, rack barcode field was not invariant";
                //     RaiseRackScannedEvent(result);
                //     return;
                //    }

                if (element_barcode != "NO TUBE" && element_barcode != "NO READ")
                {
                    RackScanResult.RackScanResultCell cell = new RackScanResult.RackScanResultCell();
                    cell.Address = address;
                    cell.Barcode = element_barcode;
                    result.Cells.Add(cell);
                    result.BarcodeToAddressMap[element_barcode] = address ;
                }

                if (element_barcode == "NO READ")
                {
                    // result.HasError = true;
                    //   result.ErrorDetail = "At least one tube failed to read.  Ensure tubes are flush and lighting is optimized.";
                }

            }

            //    if (result.RackBarcode == "NO BARCODE")
            //   {
            //      result.HasError = true;
            //       result.ErrorDetail = "No rack-level barcode could be scanned.  Ensure proper lighting and rack orientation.";
            //   }

            _result = result;

            RaiseRackScannedEvent(result);

        }

         void RaiseEventOnUIThread(Delegate theEvent, object[] args)
        {
            foreach (Delegate d in theEvent.GetInvocationList())
            {
                ISynchronizeInvoke syncer = d.Target as ISynchronizeInvoke;
                if (syncer == null)
                {
                    d.DynamicInvoke(args);
                }
                else
                {
                    syncer.BeginInvoke(d, args);  // cleanup omitted
                }
            }
        }

         void RaiseRackScannedEvent(RackScanResult result)
        {
            if (this.RackScanned != null)
                //this.RackScan(result);
                RaiseEventOnUIThread(this.RackScanned, new object[] { result });
        }

        public ScannerProfile Profile { get; private set; }

        public bool IsProcessRunning()
        {
            return (System.Diagnostics.Process.GetProcessesByName(Profile.ProcessName).Length != 0);
        }

         bool TryConnect(bool ForceNew = false)
        {

            if (_tc == null || !_tc.IsConnected || ForceNew)
            {
                if (_tc != null)
                    _tc.DataReceived -= _tc_DataReceived;
             //   MessageBox.Show("PORT: " + this.Profile.Port.ToString());
                _tc = MinimalisticTelnet.TelnetConnection2.GetLocalConnectionOnPort(this.Profile.Port);
             if (_tc!=null)
                    _tc.DataReceived += new MinimalisticTelnet.TelnetConnection2.DataReceivedEventHandler(_tc_DataReceived);
                LogEvent("New telnet connection to port " + this.Profile.Port + ((_tc != null) ? " succeeded" : " failed"));
            }

            return (_tc != null);

        }

        void _tc_DataReceived(string data)
        {
            SendResponseToLog(data);
        }

        public void SendWinsockTextDirect(string CommandText)
        {
            if (TryConnect())
                _tc.WriteLine(CommandText + System.Environment.NewLine);
            else
                GenerateError("No active connection");

        }

        public string GetWinsockText()
        {
            if (TryConnect())
                return _tc.Read();
            return null;
        }

        public RackScanResult Scan()
        {
            if (!IsProcessRunning())
            {
                LogEvent("Intellicodel process not running, starting up");
                InitializeScanner();
            }

            if (!TryConnect())
            {
                LogEvent("Connection failure, attempting to reinitialize");
                InitializeScanner();
            }

            if (!TryConnect())
            {
                LogEvent("Could not connect/start scanner profile");
                GenerateError("Could not connect/start scanner profile"); return null;
            }

            RackScanResult result = null;

            _ScanInProgress = true;
            result = ScanInternal();


           ///
           /// 
           /// for (int i = 0; i <= this.Profile.FailedScanRetryCount; i++)
          //  {
          //      LogEvent("Initiating scan attempt " + (i + 1).ToString());
         //       result = ScanInternal();
          //      result.RetryCount = i;
          //      if (!result.HasError) return result;
           // }
        //

            if (result.HasError)
                RaiseRackScannedEvent(result);


            return result;

            


        }

        public void ScanAsync()
        {
            BackgroundWorker scanWorker = new BackgroundWorker();

            scanWorker.DoWork += new DoWorkEventHandler(scanWorker_DoWork);
            scanWorker.WorkerReportsProgress = false;
            scanWorker.WorkerSupportsCancellation = false;
            scanWorker.RunWorkerAsync();
        }

        void scanWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "MyBackgroundWorkerThread";
            Scan();

        }

        void SendResponseToLog(string response)
        {
            if (response == null) return;
            response = response.Replace('\r', '\n');
            string[] responses = response.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string responseLine in responses)
                LogEvent(responseLine);

        }


        bool LoadProfileInternal()
        {
            List<string> options = new List<string>();
            options.Add("was loaded");
            options.Add("failed to load");
            string response = _tc.ReadWaitForStrings("Intellicode.Instrument.Profile.load(" + this.Profile.ProfileName + ")", options, 5000);

            if (!response.Contains("was loaded"))
            {
                LogEvent("Failed to load profile: " + this.Profile.ProfileName);
                GenerateError("Failed to load profile: " + this.Profile.ProfileName);
                return false;
            }

            LogEvent("Successfully loaded profile: " + this.Profile.ProfileName);
            return true;

        }

         RackScanResult ScanInternal()
        {

            RackScanResult result = new RackScanResult();

            //if (!LoadProfileInternal())
            //{

            //    LogEvent("Profile load failure");
            //    GenerateError("Profile load failure");
            //    return result;
            //}

            _result = null;
            List<string> options = new List<string>();
            options.Add("success");
            options.Add("fail");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            string response = _tc.ReadWaitForStrings("Intellicode.Instrument.Profile.scan", options, 60000);

            //    SendResponseToLog(response);

            result.InterfaceMessage = response;

            if (response == null) //timeout
            {
                LogEvent("Time out awaiting scanner response");
                GenerateError("Time out waiting for scanner response");
                _ScanInProgress = false;
                return result;
            }
            if (response.Contains("warning"))
            {
                LogEvent("Warning detected in scan result message");
                result.HasError = true;
                result.ErrorDetail = "Bad scan: " + response;
            }

            if (response.Contains("fail"))
            {
                LogEvent("Failure detected in scan result message");
                result.HasError = true;
                result.ErrorDetail = "Failure: " + response;
                GenerateError("Scan failure: " + response);
                _ScanInProgress = false;
                return result;
            }

            LogEvent("Success detected in scan result message");


            while (_result == null && sw.Elapsed.Seconds < 20)
            {
                System.Threading.Thread.Sleep(300);
            }

            if (_result == null)
            {
                result.HasError = true;
                result.ErrorDetail = "Timed out waiting for scan output file to appear";
                GenerateError("Timed out waiting for scan output file to appear");
                _ScanInProgress = false;
                return result;
            }

            _ScanInProgress = false;
            return _result;

        }

        public string SendAndGetResponse(string input, List<string> options)
        {
            if (TryConnect())
                return _tc.ReadWaitForStrings(input, options, 30000);
            return null;
        }

         string XRS_Startup
        {
            get { return "sys.Remote(winsock)"; }
        }

        string _XRS_File = null;
        public string XRS_File
        {
            get
            {
                if (_XRS_File == null)
                {
                    _XRS_File = Path.GetTempFileName() + ".xrs";
                    File.WriteAllText(_XRS_File, XRS_Startup);
                }
                return _XRS_File;
            }
        }


        public bool PadColumnAddress { get; set; } = true;

        public bool InitializeScanner()
        {

            if (IsProcessRunning())
            {
                LogEvent("Killing existing process");
                Kill();
                while (IsProcessRunning()) { };
                System.Threading.Thread.Sleep(3000);
            }

            //      string XRS_file = Path.Combine(this.Profile.ExeFolder, "startup.xrs");

            ///    if (!File.Exists(XRS_file))
            //     File.WriteAllText(XRS_file, XRS_Startup);

            Process proc = new Process();
            proc.StartInfo.FileName = this.Profile.FullExePath;
            proc.StartInfo.WorkingDirectory = this.Profile.ExeFolder;
            proc.StartInfo.Arguments = @"-c " + "\"" + XRS_File + "\"";
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            LogEvent("Starting new process");
            proc.Start();

            int tryCount = 0;

            while (tryCount < 10 && !TryConnect())
            {
                System.Threading.Thread.Sleep(2000);
                tryCount++;
            }



            if (!TryConnect())
            {
                LogEvent("Could not connect to listener process: InitializeScanner");
                GenerateError("Could not connect to listener process");
                return false;
            }

            System.Threading.Thread.Sleep(2000);
            List<string> options = new List<string>();
            options.Add("success");
            options.Add("fail");
            string response = null;

            try
            {
                response = _tc.ReadWaitForStrings("Intellicode.Instrument.use(" + this.Profile.InstrumentName + ")", options, 30000);
                //  SendResponseToLog(response);
            }
            catch (Exception)
            {
                try
                {
                    if (!TryConnect(true))
                    {
                        LogEvent("Could not connect to listener process: InitializeScanner");
                        GenerateError("Could not connect to listener process");
                        return false;
                    }
                    response = _tc.ReadWaitForStrings("Intellicode.Instrument.use(" + this.Profile.InstrumentName + ")", options, 30000);

                }
                catch (Exception ex2) 
                {
                    LogEvent("Unexpected  exception: " + ex2.Message);
                    GenerateError(ex2.Message);
                    return false;
                }
            }

            if (response == null)
            {
                LogEvent("Interface timeout");
                GenerateError("Interface timeout");
                return false;
            }

            if (!response.Contains("success"))
            {
                LogEvent("Failed to connect to instrument: " + this.Profile.InstrumentName);
                GenerateError("Failed to connect to instrument: " + this.Profile.InstrumentName);
                return false;
            }

            //     this.CurrentState = State.InstrumentLoaded;
            LogEvent("Successfully connected to instrument: " + this.Profile.InstrumentName);
            options.Clear();

            options.Add("was loaded");
            options.Add("failed to load");
            response = _tc.ReadWaitForStrings("Intellicode.Instrument.Profile.load(" + this.Profile.ProfileName + ")", options, 5000);

            if (!response.Contains("was loaded"))
            {
                LogEvent("Failed to load profile: " + this.Profile.ProfileName);
                GenerateError("Failed to load profile: " + this.Profile.ProfileName);
                return false;
            }

            LogEvent("Successfully loaded profile: " + this.Profile.ProfileName);
            return true;

        }

        void GenerateError(string ErrorMessage)
        {
            this.ErrorMessage = ErrorMessage;
        }

        public void Kill()
        {
            foreach (Process p in System.Diagnostics.Process.GetProcessesByName(Profile.ProcessName))
                p.Kill();
        }
    }
}
