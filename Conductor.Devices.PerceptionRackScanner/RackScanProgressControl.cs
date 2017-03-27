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
    public partial class RackScanProgressControl : UserControl
    {
        public RackScanProgressControl()
        {
            InitializeComponent();
        }

        public void Bind(RackScanner scanner)
        {

            scanner.RackScannerLogEvent += new RackScanner.RackScannerLogEventHandler(scanner_RackScannerLogEvent);


        }
        public void Clear()
        {

            this.lstLog.Items.Clear();
            this.progressBar1.Value = 0;
            this.progressBar1.Visible = false;

        }
        void scanner_RackScannerLogEvent(string message)
        {
            this.lstLog.Items.Add(message);
            this.lstLog.SelectedIndex = lstLog.Items.Count - 1;
            lstLog.TopIndex = lstLog.Items.Count - 1;

            int ToGo = 100 - this.progressBar1.Value;
            int step = ToGo / 2;
            this.progressBar1.Value += step;

        }

        public bool ShowProgress { get { return this.progressBar1.Visible; } set { this.progressBar1.Visible = value; } }





    }
}
