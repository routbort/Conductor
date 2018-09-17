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
    public partial class RackScanProgressControl : UserControl
    {
        public RackScanProgressControl()
        {
            InitializeComponent();
        }

        public void Bind(IRackScanner scanner)
        {
     

            scanner.RackScannerLogEvent += Scanner_RackScannerLogEvent;

        }

        private void Scanner_RackScannerLogEvent(object sender, RackScanEventLogEntry e)
        {
            this.lstLog.Items.Add(e.When.ToLongTimeString() + ": " + e.Message);
            this.lstLog.SelectedIndex = lstLog.Items.Count - 1;
            lstLog.TopIndex = lstLog.Items.Count - 1;
            int ToGo = 100 - this.progressBar1.Value;
            int step = ToGo / 2;
            this.progressBar1.Value += step;
        }

        public void Clear()
        {

            this.lstLog.Items.Clear();
            this.progressBar1.Value = 0;
            this.progressBar1.Visible = false;

        }
 

        public bool ShowProgress { get { return this.progressBar1.Visible; } set { this.progressBar1.Visible = value; } }





    }
}
