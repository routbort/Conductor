using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conductor.Devices.PerceptionRackScanner
{
    public class RackScanResult
    {

        public RackScanResult()
        {
            Cells = new List<RackScanResultCell>();
            BarcodeToAddressMap = new Dictionary<string, string>();
            HasError = false;
        }

        public class RackScanResultCell
        {

            string _Address = null;

            public string Address
            {
                get { return _Address; }
                set
                {
                    _Address = value;
                }
            }
            public string Barcode { get; set; }


        }


        public int RetryCount { get; set; }
        public string InterfaceMessage { get; set; }
        public string RackBarcode { get; set; }
        public List<RackScanResultCell> Cells { get; private set; }
        public Dictionary<string, string> BarcodeToAddressMap { get; private set; }
        public bool HasError { get; set; }
        public string ErrorDetail { get; set; }

    }
}
