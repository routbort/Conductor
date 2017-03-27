using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Conductor.Devices.BarcodeScanner
{
    public partial class ConnectDevice : Form
    {
        public BarCodeWatcher MyBarCodeWatcher;

        public ConnectDevice()
        {
            InitializeComponent();
        }

        private void ConnectDevice_Load(object sender, EventArgs e)
        {
            UpdateListDevice();
        }

        private void UpdateListDevice()
        {
            this.lstDevices.Items.Clear();
                    foreach (HidDevice EachScanner in MyBarCodeWatcher._Scanners)
            {
                this.lstDevices.Items.Add(EachScanner);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            MyBarCodeWatcher.StartReading();
            UpdateListDevice();
        }

    }
}
