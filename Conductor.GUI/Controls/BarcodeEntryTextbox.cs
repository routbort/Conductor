using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Conductor.GUI
{
    public class BarcodeEntryTextbox : System.Windows.Forms.TextBox
    {

        public BarcodeEntryTextbox() : base()
        {

            this.GotFocus += BarcodeEntryTextbox_GotFocus;
            this.LostFocus += BarcodeEntryTextbox_LostFocus;
            this.KeyDown += BarcodeEntryTextbox_KeyDown;

        }

        public class BarcodeScannedEventArgs : EventArgs
        {
            public string Barcode { get; set; }
            public BarcodeScannedEventArgs(string Barcode) { this.Barcode = Barcode; }
        }

        public event EventHandler<BarcodeScannedEventArgs> BarcodeScanned;

        private void BarcodeEntryTextbox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                BarcodeScanned?.Invoke(this, new BarcodeScannedEventArgs(this.Text));
                this.Text = "";
            }
        }

        private void BarcodeEntryTextbox_LostFocus(object sender, EventArgs e)
        {
            this.BackColor = System.Drawing.Color.White;
        }

        private void BarcodeEntryTextbox_GotFocus(object sender, EventArgs e)
        {
            this.BackColor = System.Drawing.Color.Green;
            this.SelectAll();
        }



    }
}
