using Conductor.Devices.BarcodeScanner;
using Conductor.Devices.RackScanner;
using System;
using System.Collections.Generic;
using System.Drawing;
using Conductor.GUI.Examples;
using System.Windows.Forms;
using System.Management;

namespace QuickExample
{
    public partial class Form1 : Form
    {
        delegate void SetCodeReadCallback(Code barcode);
        BarCodeWatcher _BCW;
        string _CurrentRackCode = null;
        Dictionary<string, Dictionary<string, string>> _tubes = new Dictionary<string, Dictionary<string, string>>();

        public Form1()
        {
            InitializeComponent();
            this.simpleRackScanControl1.RackScanned += SimpleRackScanControl1_RackScanned;
            _BCW = new BarCodeWatcher();
            _BCW.CodeRead += _BCW_CodeRead;
            ShowMessage("Select store or check mode");
        }

        private void SimpleRackScanControl1_RackScanned(object sender, RackScanResult e)
        {
            this.simpleRackScanControl1.Visible = false;
            if (e.HasError)
            {
                this.cartesianGrid1.Visible = false;
                this.ShowMessage(e.ErrorDetail, true);
                return;
            }
            this.cartesianGrid1.Visible = true;
            this.cartesianGrid1.Text = (e.RackBarcode != null && e.RackBarcode !="")? this.txtBarcode.Text + " (" + e.RackBarcode + ")": this.txtBarcode.Text;
            Dictionary<string, string> currentTubes = new Dictionary<string, string>();

            List<Sample> tubes = new List<Sample>();
            foreach (var cell in e.Cells)
            {
                Sample tube = new Sample();
                tube.CurrentCartesianAddress = cell.Address;
                currentTubes[cell.Address] = cell.Barcode;
                tube.GridCellLabel = cell.Barcode;
                tube.GridCellBackColor = (this.listBox1.SelectedIndex == 0) ? this.cartesianGrid1.BackColor : Color.LightGreen;
                tubes.Add(tube);
            }

            this.cartesianGrid1.DataSource = tubes;
            if (listBox1.SelectedIndex == 0)
            {
                _tubes[_CurrentRackCode] = currentTubes;
                ShowMessage("Rack inventory storage is complete" + System.Environment.NewLine + "Ready for the next rack");
                SoundHelper.PlayWaveResource("WorkflowProgress.wav");
            }
            else
            {
                bool Error = false;
                Dictionary<string, string> storedTubes = _tubes[_CurrentRackCode];
                foreach (string address in this.cartesianGrid1.GetAddressesInFillOrder())
                {
                    if (storedTubes.ContainsKey(address) && currentTubes.ContainsKey(address) && storedTubes[address] != currentTubes[address])
                    {
                        this.cartesianGrid1.SetOverlay(address, Color.Red, "WRONG TUBE");
                        Error = true;
                    }
                    if (storedTubes.ContainsKey(address) && !currentTubes.ContainsKey(address))
                    {
                        this.cartesianGrid1.SetOverlay(address, Color.Red, "MISSING");
                        Error = true;
                    }
                    if (!storedTubes.ContainsKey(address) && currentTubes.ContainsKey(address))
                    {
                        this.cartesianGrid1.SetOverlay(address, Color.Red, "EXTRA");
                        Error = true;
                    }
                }

                if (Error)
                {
                    SoundHelper.PlayWaveResource("MajorError.wav");
                    ShowMessage("Error detected - Check tubes and try again", true);
                }
                else
                {
                    SoundHelper.PlayWaveResource("WorkflowProgress.wav");
                    ShowMessage("All tubes match" + System.Environment.NewLine + "Ready for the next rack");
                }

            }
        }

        void HandleBarcodeScan(Code barcode)
        {
            if (this.listBox1.SelectedIndex == -1)
            {
                ShowMessage("Select store or check mode", true);
                SoundHelper.PlayWaveResource("MinorError.wav");
                return;
            }

            if (this.simpleRackScanControl1.RackScanner == null)

            {
                ShowMessage("Select a scanner type", true);
                SoundHelper.PlayWaveResource("MinorError.wav");
                return;
            }

            this.cartesianGrid1.DataSource = null;
            this.Refresh();

            if (listBox1.SelectedIndex == 1 && !_tubes.ContainsKey(barcode.TextData))
            {
                ShowMessage("Rack " + _CurrentRackCode + " has not yet been inventoried", true);
                SoundHelper.PlayWaveResource("MinorError.wav");
                return;
            }

            _CurrentRackCode = barcode.TextData;
            this.lblMessage.Visible = false;
            this.simpleRackScanControl1.Visible = true;
            this.simpleRackScanControl1.Scan();
        }

        private void _BCW_CodeRead(Code barcode)
        {
            Invoke((MethodInvoker)delegate
            {
                HandleBarcodeScan(barcode);
            });
        }


        private void simpleRackScanControl1_RackScanned(RackScanResult data)
        {
            Invoke((MethodInvoker)delegate
            {
                SimpleRackScanControl1_RackScanned(this, data);
            });

        }

        void ShowMessage(string Message, bool Error = false)
        {
            this.lblMessage.Text = Message;
            this.lblMessage.ForeColor = (Error) ? Color.Red : Color.Black;
            this.lblMessage.Visible = true;

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == 0)
            {
                //testing that DLL change is ignored
                ShowMessage("Store into inventory " + System.Environment.NewLine + "Place rack on scanner and scan the rack bar code");
            }
            else
            {
                ShowMessage("Check existing plate map" + System.Environment.NewLine + "Place rack on scanner and scan the rack bar code");
            }
            SoundHelper.PlayWaveResource("WorkflowInitiated.wav");
            this.cartesianGrid1.DataSource = null;
            this.Refresh();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            Code bc = new Code(this.txtBarcode.Text, Symbology.Code128);
            this.HandleBarcodeScan(bc);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        Conductor.Devices.RackScanner.ZiathRackScanner _rs = null;





        private void rbZiath_CheckedChanged(object sender, EventArgs e)
        {
            if (rbZiath.Checked)
            {
                if (!ZiathRackScanner.IsZiathScannerAttached())
                    MessageBox.Show("No Ziath scanner detected - scanning may fail"); 
                ZiathRackScanner zr = new ZiathRackScanner();
                zr.Profile.ProfileName = "24";
                this.simpleRackScanControl1.Bind(zr);
            }
        }

        private void rbFluidX_CheckedChanged(object sender, EventArgs e)
        {
            if (rbFluidX.Checked)
            {
                if (!FluidXRackScanner.IsFluidXScannerAttached())
                    MessageBox.Show("No FluidX scanner detected - scanning may fail");
                FluidXRackScanner fr = new FluidXRackScanner();
                this.simpleRackScanControl1.Bind(fr);
            }
        }

    }
}
