using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Conductor.Devices.PerceptionRackScanner
{
    public partial class SimpleRackScanControl : UserControl
    {
        public SimpleRackScanControl()
        {
            InitializeComponent();
            ScannerProfile profile = new ScannerProfile();
            profile.Conversion = ScannerProfile.FlipType.Flip96;
            _RackScanner = new RackScanner(profile);
            _RackScanner.RackScanned += RaiseRackScannedEvent;

        }
        public delegate void RackScannedEventHandler(RackScanResult data);
        public event RackScannedEventHandler RackScanned;

        bool _IsBound = false;

        Conductor.Devices.PerceptionRackScanner.RackScanner _RackScanner = null;

        public RackScanner RackScanner { get { return _RackScanner; } }

        public void Scan()
        {
            if (!_IsBound)
            {
                this.rackScanLogViewer1.Bind(_RackScanner);
                _IsBound = true;
            }
            this.rackScanLogViewer1.Clear();
            this.rackScanLogViewer1.ShowProgress = true;
            _RackScanner.ScanAsync();
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

            Invoke((MethodInvoker)delegate
            {
                this.rackScanLogViewer1.ShowProgress = false;
            });


            if (this.RackScanned != null)
                //this.RackScan(result);
                RaiseEventOnUIThread(this.RackScanned, new object[] { result });
        }



    }
}
