using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Conductor.Devices.RackScanner
{
    public partial class SimpleRackScanControl : UserControl
    {
        public SimpleRackScanControl()
        {
            InitializeComponent();

            //      _RackScanner = new ZiathRackScanner(profile);
            //   _RackScanner.RackScanned += _RackScanner_RackScanned;
        }


        public void Bind(IRackScanner RackScanner)
        {

            if (_RackScanner != null)
                _RackScanner.RackScanned -= _RackScanner_RackScanned;

            _RackScanner = RackScanner;
            _RackScanner.RackScanned += _RackScanner_RackScanned;
            this.rackScanLogViewer1.Bind(_RackScanner);

        }

        private void _RackScanner_RackScanned(object sender, RackScanResult e)
        {
            Invoke((MethodInvoker)delegate
            {
                this.rackScanLogViewer1.ShowProgress = false;
            });


            if (this.RackScanned != null)
                RaiseEventOnUIThread(this.RackScanned, new object[] { this, e });
        }
        public event EventHandler<RackScanResult> RackScanned;

        Conductor.Devices.RackScanner.IRackScanner _RackScanner = null;

        public IRackScanner RackScanner { get { return _RackScanner; } }

        public void Scan()
        {

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

    }
}
