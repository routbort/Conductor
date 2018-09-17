using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Threading;
using Microsoft.Win32;
using System.Management;

namespace Conductor.Devices.RackScanner
{
    public class ZiathRackScanner : IRackScanner
    {
        #region Static

        public static Dictionary<string, string> ZIATH_DEVICES =
             new Dictionary<string, string>() { { "Mirage", "VID_0BDA&PID_8153" } };

        public static bool IsZiathScannerAttached()
        {

            foreach (var deviceName in ZIATH_DEVICES.Keys)
                if (Conductor.Components.USBHelper.IsSpecificDeviceAvailable(ZIATH_DEVICES[deviceName]))
                    return true;
            return false;
        }

        public static void KillServer()
        {
            foreach (Process p in System.Diagnostics.Process.GetProcessesByName("java"))
            {
                Process parent = Conductor.Components.ProcessHelper.GetParent(p);
                if (parent.ProcessName == "server")
                {
                    parent.Kill();
                    p.Kill();
                }
            }
        }

        #endregion

        #region IRackScanner

        public event EventHandler<RackScanResult> RackScanned;
        public event EventHandler<RackScanEventLogEntry> RackScannerLogEvent;

        #endregion

        #region Internal

        void LogEvent(string message)
        {
            RackScanEventLogEntry entry = new RackScanEventLogEntry(message);
            if (this.RackScannerLogEvent != null)
                RaiseEventOnUIThread(this.RackScannerLogEvent, new object[] { this, entry });
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
                RaiseEventOnUIThread(this.RackScanned, new object[] { this, result });
        }

        Conductor.Components.TelnetConnection InitializeServer()
        {
            LogEvent("Starting new process");
            Process process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.Arguments = "-s";
            process.StartInfo.FileName = Path.Combine(ExePath(), this.Profile.ExeName);
            if (!File.Exists(process.StartInfo.FileName))
                return null;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            int tryCount = 1;
            int maxTry = 10;
            Conductor.Components.TelnetConnection connection = null;
            while (tryCount < maxTry)
            {
                connection = GetConnection(false);
                if (connection != null)
                    return connection;
                System.Threading.Thread.Sleep(500);
                tryCount++;
                LogEvent("Waiting for server to become available...cycle " + tryCount.ToString());
            }
            LogEvent("Unable to acquire a connection within " + maxTry.ToString() + " attempts.");
            return null;
        }

        Conductor.Components.TelnetConnection GetConnection(bool StartServerIfNeeded = true)
        {
            Conductor.Components.TelnetConnection connection = null;

            if (!IsProcessRunning() && StartServerIfNeeded)
            {

                LogEvent("DataPaq server process not running, starting up");
                connection = InitializeServer();
            }
            else
            {
                connection = Conductor.Components.TelnetConnection.GetLocalConnectionOnPort(this.Profile.Port);
            }

            if (connection == null)
            {
                string error = "Unable to obtain a connection to the DataPaq.  Ensure software is installed and configured.";
                LogEvent(error);
                GenerateError(error);
            }
            else
            {
                string startup = connection.Read();
                if (startup.Contains("ERR"))
                {
                    LogEvent("Communication error: " + startup);
                    GenerateError("Communication error: " + startup);
                    connection = null;
                }
                else
                    LogEvent("Connection response:" + startup);
            }

            return connection;
        }

        string SendCommand(string command)
        {
            var conn = GetConnection();
            LogEvent("Sending command: " + command);
            string data = conn.ReadWaitForStrings(command, new List<string>() { "OK", "ERR" }, 1000);
            data = data.Replace("OK\r\n", "");
            conn.WriteLine("CLOSE");
            conn.Close();
            return data;
        }

        void scanWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "scanBackgroundWorker";
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

        void GenerateError(string ErrorMessage)
        {
            this.ErrorMessage = ErrorMessage;
        }

        RackScanResult GetRackScanResultFromData(string data)
        {
            RackScanResult result = new RackScanner.RackScanResult();
            if (data.StartsWith("Date,"))
            {
                char[] div = new char[] { ',' };
                foreach (var line in data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] items = line.Split(div);
                    if (items != null && items.Length == 7)
                    {
                        string rack_barcode = items[2];
                        string row = items[3];
                        int colInt = Convert.ToInt32(items[4]);

                        string col = colInt.ToString("D2");
                        string barcode = items[5];
                        result.RackBarcode = rack_barcode;
                        if (barcode != "EMPTY")
                        {
                            RackScanResult.RackScanResultCell cell = new RackScanResult.RackScanResultCell();
                            cell.Address = row + col;
                            cell.Barcode = barcode;
                            result.Cells.Add(cell);
                            result.BarcodeToAddressMap[barcode] = cell.Address;
                        }
                    }
                }
            }
            return result;
        }

        void RebuildRackScanResult(RackScanResult result)
        {
            result.BarcodeToAddressMap.Clear();
            foreach (var c in result.Cells)
                result.BarcodeToAddressMap[c.Barcode] = c.Address;
        }

        RackScanResult MergeRackScanResults(RackScanResult r1, RackScanResult r2)
        {

            for (int i = 0; i < r1.Cells.Count; i++)
                if (r1.Cells[i].Barcode == "TRIAL")
                    r1.Cells[i].Barcode = r2.Cells[i].Barcode;
            return r1;
        }

        bool RackScanResultIsComplete(RackScanResult r)
        {
            foreach (var c in r.Cells)
                if (c.Barcode == "TRIAL")
                    return false;
            return true;
        }

        #endregion

        #region Public

        public string ErrorMessage { get; private set; }

        public ZiathRackScanner() { this.Profile = new ZiathScannerProfile(); }

        public bool SoftwareInstalled { get { return (this.ExePath() != ""); } }

        public bool SupportedDeviceInstalled
        {
            get
            {
                foreach (string deviceName in ZIATH_DEVICES.Keys)
                    return true;
                return false;
            }
        }

        public ZiathScannerProfile Profile { get; private set; }

        public bool IsProcessRunning()
        {
            return (System.Diagnostics.Process.GetProcessesByName(Profile.ProcessName).Length != 0);
        }

        public string GetVersion()
        {
            return SendCommand("VERSION");
        }

        public List<RackProfile> GetRackProfiles()

        {
            var output = new List<RackProfile>();
            string data = SendCommand("GET_UIDS");
            var uids = data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var uid in uids)
            {
                var items = uid.Split(new char[] { '|' });
                if (items.Length == 3)
                    output.Add(new RackProfile() { Name = items[2], UID = Convert.ToInt32(items[0]) });
            }

            return output;
        }

        public RackScanResult Scan()
        {
            RackScanResult result = new RackScanResult();
            var conn = GetConnection();
            LogEvent("Sending scan command");
            string data = conn.ReadWaitForStrings("SCAN " + this.Profile.ProfileName + " TEXT", new List<string>() { "OK", "ERR" }, 1000);

            if (data.Contains("ERR"))
            {
                data = data.Replace("OK\r\n", "");
                result.HasError = true;
                result.ErrorDetail = data;
                GenerateError(data);
                LogEvent("Scanner error: " + data);
                return result;
            }

            LogEvent("Scan command acknowledged.  Acquiring ...");
            data = conn.ReadWaitForStrings("", new List<string>() { "OK", "ERR" }, 5000);
            if (data.Contains("ERR"))
            {
                data = data.Replace("OK\r\n", "");
                result.HasError = true;
                result.ErrorDetail = data;
                GenerateError(data);
                LogEvent("Scanner error: " + data);
                return result;
            }

            LogEvent("Processing scan data");
            if (data.StartsWith("Date,"))
                result = GetRackScanResultFromData(data);
            conn.WriteLine("CLOSE");
            conn.Close();
            RaiseRackScannedEvent(result);
            return result;

        }

        public RackScanResult AdvancedScan()
        {

            RackScanResult result = new RackScanResult();
            var conn = GetConnection();

            if (conn == null)
            {

                result.HasError = true;
                result.ErrorDetail = "Could not get a connection to Ziath device.  Is DataPaq installed and device attached?";
                RaiseRackScannedEvent(result);
                return result;
            }

            int tries = 1;
            LogEvent("Sending scan command " + tries.ToString());
            string data = conn.ReadWaitForStrings("SCAN " + this.Profile.ProfileName + " TEXT", new List<string>() { "OK", "ERR" }, 1000);
            if (data.Contains("ERR"))
            {
                data = data.Replace("OK\r\n", "");
                result.HasError = true;
                result.ErrorDetail = data;
                GenerateError(data);
                LogEvent("Scanner error: " + data);
                RaiseRackScannedEvent(result);
                return result;
            }

            LogEvent("Scan command acknowledged.  Acquiring ...");

            data = conn.ReadWaitForStrings("", new List<string>() { "OK", "ERR" }, 5000);

            if (data.Contains("ERR"))
            {
                data = data.Replace("OK\r\n", "");
                result.HasError = true;
                result.ErrorDetail = data;
                GenerateError(data);
                LogEvent("Scanner error: " + data);
                RaiseRackScannedEvent(result);
                return result;
            }

            LogEvent("Processing scan data");

            result = GetRackScanResultFromData(data);

            while (!RackScanResultIsComplete(result))
            {
                tries++;
                LogEvent("Sending scan command " + tries.ToString());
                data = conn.ReadWaitForStrings("SCAN " + this.Profile.ProfileName + " TEXT", new List<string>() { "OK", "ERR" }, 1000);

                if (data.Contains("ERR"))
                {
                    data = data.Replace("OK\r\n", "");
                    result.HasError = true;
                    result.ErrorDetail = data;
                    GenerateError(data);
                    LogEvent("Scanner error: " + data);
                    return result;
                }

                LogEvent("Scan command acknowledged.  Acquiring ...");
                data = conn.ReadWaitForStrings("", new List<string>() { "OK", "ERR" }, 5000);
                if (data.Contains("ERR"))
                {
                    data = data.Replace("OK\r\n", "");
                    result.HasError = true;
                    result.ErrorDetail = data;
                    GenerateError(data);
                    LogEvent("Scanner error: " + data);
                    return result;
                }
                LogEvent("Processing scan data " + tries.ToString());
                RackScanResult result2 = GetRackScanResultFromData(data);
                result = MergeRackScanResults(result, result2);
            }
            conn.WriteLine("CLOSE");
            conn.Close();
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

        public string ExePath()
        {

            string keyName1 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\DataPaq_is1";
            string keyName2 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\DataPaq_is1";
            string valueName = "InstallLocation";
            string path = "";
            try
            {
                path = Environment.GetEnvironmentVariable("PROGRAMFILES(X86)") == null ? Registry.GetValue(keyName2, valueName, (object)null).ToString() : Registry.GetValue(keyName1, valueName, (object)null).ToString();
            }
            catch { }
            return path;
        }

        #endregion

        #region Contained classes

        public class RackProfile
        {
            public int UID { get; set; }
            public string Name { get; set; }
        }

        #endregion

    }

}
