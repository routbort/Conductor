using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///<Summary>
/// HidWatcher2 BarCodeWatcher
/// Define Symbology + PartialSegment or not + Get list of Scanner + process barcode scanned.
///</Summary>

namespace Conductor.Devices.BarcodeScanner
{

    public enum Symbology
    {
        Code128,
        Code39,
        Code2of5,
        Codabar,
        DataMatrix,
        EAN8,
        EAN13,
        UPCA,
        UPCE,
        PDF417,
        PostNet37,
        Undefined
    }

    public class BarCodeWatcher
    {

        public delegate void CodeReadHandler(Code barcode);
        public event CodeReadHandler CodeRead;

        public static Code GetCode(byte[] input)
        {
            Symbology sym;
            int bufferLength = input[0];
            if (bufferLength == 0) return null;
            bool isNumeric = input[bufferLength - 1] != 11;
            int offset = (isNumeric) ? 1 : 2;
            int symbologyDigit = input[bufferLength - offset];

            switch (symbologyDigit)
            {
                case 24:
                    sym = (isNumeric) ? Symbology.Undefined : Symbology.Code128;
                    break;
                case 10:
                    sym = (isNumeric) ? Symbology.UPCE : Symbology.Code39;
                    break;
                case 13:
                    sym = (isNumeric) ? Symbology.UPCA : Symbology.Code2of5;
                    break;
                case 14:
                    sym = (isNumeric) ? Symbology.Undefined : Symbology.Codabar;
                    break;
                case 22:
                    sym = (isNumeric) ? Symbology.EAN13 : Symbology.Undefined;
                    break;
                case 37:
                    sym = (isNumeric) ? Symbology.Undefined : Symbology.PostNet37;
                    break;
                case 46:
                    sym = (isNumeric) ? Symbology.Undefined : Symbology.PDF417;
                    break;
                case 255:
                    sym = (isNumeric) ? Symbology.Undefined : Symbology.DataMatrix;
                    break;
                case 12:
                    sym = (isNumeric) ? Symbology.Undefined : Symbology.EAN8;
                    break;
                default:
                    sym = Symbology.Undefined;
                    break;
            }

            int CodeBufferEnd = (isNumeric) ? bufferLength - 2 : bufferLength - 4;
            //int BarcodeNumCharHoldCapacity = ((isNumeric) ? input.Length - 2 : input.Length - 4) - 3;

            string output = null;
            
            bool BufferExceeded = bufferLength >= (input.Length);

            if (bufferLength == input.Length) //Handle of Long Barcode
            {
                //Below logic was giving wrong info on testing EAN8 barcode. So, I put it into an if statement, will only use when necessary. 
                //We can either use it for just potential long symbologies or just use it when the array is full; we can easily look at array.length, see all used or not.

                BufferExceeded = (input[bufferLength - 3] != 0); // This logic is tested to work with Code 128, DataMatrix, PDF417. (Numeric and alphanumeric). The rest will not have long barcode values. 
            }


            for (int i = 4; i <= CodeBufferEnd; i++)
            {
                char nextChar = (isNumeric && sym != Symbology.UPCE) ? Convert.ToChar(input[i].ToString()) : (char)input[i];
                output += nextChar.ToString();
            }

            return new Code(output, sym, BufferExceeded);
        }

        private List<int> checkValues;
        private HidDevice _scanner = null;
        private HidDevice Scanner
        {
            get
            {
                return _scanner;
            }
            set
            {
                _scanner = value;
                _Scanners.Add(_scanner);
            }
        }

        public System.Collections.Generic.List<HidDevice> _Scanners = new System.Collections.Generic.List<HidDevice>();

        public BarCodeWatcher(List<int> CheckUsageValues)
        {
            checkValues = CheckUsageValues;
            StartReading();
        }

        public BarCodeWatcher()
        {
            checkValues = new List<int>();
            checkValues.Add(19200);
            checkValues.Add(18944);
            StartReading();

        }

        public void StartReading()
        { 
            //TODO testing to check other Symbol bar code scanners in correct mode - 
            //Question - is using Usage filter appropriate/reliable?
            HidDevice[] devices = HidDevices.Enumerate().ToArray();

            //Before removing disconnected one: We won't be able to remove while it's in a foor loop, so will make a copy
            System.Collections.Generic.List<HidDevice> _ScannersCopy = new System.Collections.Generic.List<HidDevice>(_Scanners);
            //Remove if Scanner Removed (if not exist in latest HidDevice read)
            foreach (HidDevice scan in _ScannersCopy)
            {
                bool blnScanExist = false;
                foreach (var device in devices)
                {
                    if (device.DevicePath == scan.DevicePath)
                    {
                        blnScanExist = true;
                        break;
                    }
                }
                if (!blnScanExist) _Scanners.Remove(scan); //Remove from main Scanner list
            }


            //We only want to connect scanners when they are at "Handheld IBM USB Mode" so we will use below filter so that they don't connect other modes such as "Keyboard Wedge", etc.
            //List<int> checkValues = new List<int> { 19200, 18944 };

            foreach (var device in devices)
                //if (device.ProductName == "Symbol Bar Code Scanner" && (device.Capabilities.Usage == 19200 ||device.Capabilities.Usage==18944))
                //if (device.ProductName == "Symbol Bar Code Scanner" && (checkValues.Contains(device.Capabilities.Usage)))
                if ((device.ProductName != null) && (device.ProductName.StartsWith("Symbol Bar Code Scanner")) && (checkValues.Contains(device.Capabilities.Usage)) && (! _Scanners.Contains(device)))
                {
                    //Before we add, need to make sure if already exist - do not add again
                    bool blnScanExist = false;
                    foreach (HidDevice scan in _Scanners)
                    {
                        if (device.DevicePath == scan.DevicePath)
                        {
                            blnScanExist = true;
                            break; 
                        }
                    }
                    
                    if (!blnScanExist)
                    {
                     Scanner = device;
                    _scanner.OpenDevice();

                    _scanner.Inserted += new InsertedEventHandler(_scanner_Inserted);
                    _scanner.Removed += new RemovedEventHandler(_scanner_Removed);
                    _scanner.MonitorDeviceEvents = true;
                    _scanner.ReadReport(ReadProcess);
                    //System.Diagnostics.Debug.WriteLine("Usage:" + _scanner.Capabilities.Usage.ToString());
                    //System.Diagnostics.Debug.WriteLine("Usage page:" + _scanner.Capabilities.UsagePage.ToString());
                    //break;
                    }

                }
        }

        private bool _Active = true;

        void _scanner_Inserted()
        {
            _Active = true;
            _scanner.ReadReport(ReadProcess);
        }

        void _scanner_Removed()
        {
            _Active = false;
        }

        const int SEQUENTIAL_READ_INTERVAL_MAX = 500; //No need to read more frequent then half a second
        const bool RAISE_CODE_FOR_SINGLE_CHARS = false; //Do we have any barcodes that just carry single character?! Doesn't seem like so omit those
        private int _LastCodeScannedTickcount = 0; //We will use this to compare to see to handle SwallowRead and will store the last barcode scan's time
        private System.Collections.Generic.List<string> _Results = new System.Collections.Generic.List<string>(); //This became necessary on long barcodes. Multiple threads hit the code at the same time so storing one by one outside
        private Code CurrentCode = default(Code);      //Current Code
        private Code LastCodeScanned = default(Code);  //Previous Code

        private void ReadProcess(HidReport report)
        {
            if (CodeRead != null)
            {
                CurrentCode = GetCode(report.Data);

                if (CurrentCode != null)
                {

                    if ((LastCodeScanned != null && LastCodeScanned.TextData == CurrentCode.TextData) && (System.Environment.TickCount - _LastCodeScannedTickcount < SEQUENTIAL_READ_INTERVAL_MAX) && LastCodeScanned.Symbology == CurrentCode.Symbology)
                    {
                        //Swallow dead read
                        //Debug.Print("Dead read detected reading"); 
                        Console.WriteLine("Dead read detected reading");
                    }
                    else
                    {
                        //Add it to to our string array
                        _Results.Add(CurrentCode.TextData);

                        //Console.WriteLine("myThreadCount=" + report._myThreadCount);

                        //Multithread on long barcodes so we will form the final result by concatenating
                        string strReturn = "";
                        foreach (var s in _Results)
                        {
                            strReturn += s;
                        }

                        if (RAISE_CODE_FOR_SINGLE_CHARS || (CurrentCode.TextData.Length > 1 || strReturn.Length > 1))
                        { //The addition of OR is for 64+1 condition - compatibility with long barcodes

                            if (!CurrentCode.PartialSegment)
                            {
                                CurrentCode.TextData = strReturn;
                                if (_Results.Count != 0)
                                {
                                    _Results.Clear();
                                    

                                    LastCodeScanned = CurrentCode;
                                    _LastCodeScannedTickcount = System.Environment.TickCount;
                                    CodeRead(CurrentCode);
                                }
                            }
                        }
                    }
                }
                else
                    System.Diagnostics.Debug.WriteLine("empty code");
            }

            if (_Active)
            {
                foreach (var s in _Scanners)
               {
                   s.ReadReport(ReadProcess);
               }
                
            }         
        }

        public void ShowConnectDeviceForm()
        {
            ConnectDevice BrowseForm = new ConnectDevice();
            BrowseForm.MyBarCodeWatcher = this;
            BrowseForm.Show();
        }

    }

}