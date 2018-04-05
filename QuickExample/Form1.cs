using Conductor.Devices.BarcodeScanner;
using Conductor.Devices.PerceptionRackScanner;
using System;
using System.Collections.Generic;
using System.Drawing;
using Conductor.GUI.Examples;
using System.Windows.Forms;

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
            this.simpleRackScanControl1.RackScanner.Profile.ProfileName = "96 2.xtprof";
            this.simpleRackScanControl1.RackScanner.RackScanned += simpleRackScanControl1_RackScanned;
            _BCW = new BarCodeWatcher();
            _BCW.CodeRead += _BCW_CodeRead;
            ShowMessage("Select store or check mode");
        }

        void HandleBarcodeScan(Code barcode)
        {
            if (this.listBox1.SelectedIndex == -1)
            {
                ShowMessage("Select store or check mode", true);
                SoundHelper.PlayWaveResource("MinorError.wav");
                return;
            }

            this.cartesianGrid1.DataSource = null;
            this.Refresh();

            if (listBox1.SelectedIndex == 1 && !_tubes.ContainsKey(barcode.TextData))
            {
                ShowMessage ("Rack " + _CurrentRackCode + " has not yet been inventoried", true);
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

        void HandleRackScan(RackScanResult data)
        {
            this.simpleRackScanControl1.Visible = false;

            Dictionary<string, string> currentTubes = new Dictionary<string, string>();

            List<Sample> tubes = new List<Sample>();
            foreach (var cell in data.Cells)
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

        private void simpleRackScanControl1_RackScanned(RackScanResult data)
        {
            Invoke((MethodInvoker)delegate
            {
                HandleRackScan(data);
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
    }
}
