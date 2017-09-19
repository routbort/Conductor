using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Conductor.Devices.CABPrinter
{
    public partial class frmTest : Form
    {
        public frmTest()
        {
            InitializeComponent();
            this.txtTemplate.Text = CABLabelHelper.RackLabelTemplateExample;
            SimpleSubstitutionHolder holder = new SimpleSubstitutionHolder();
            this.propertyGrid1.SelectedObject = holder;
        }

        private void cmdPrint_Click(object sender, EventArgs e)
        {
           
                string ToPrint = CABLabelHelper.GetLabelData(this.txtTemplate.Text,
                (this.propertyGrid1.SelectedObject as SimpleSubstitutionHolder).GetReplacements());

                ToPrint = CABLabelHelper.GetAdjustedLabel(ToPrint, (float) udX.Value, (float) udY.Value);

            CABLabelHelper.Print(
                  ToPrint,
                 this.txtIPAddress.Text, 
                 (int)this.upCopyNumber.Value);

            if (this.chkFormFeed.Checked)
                CABLabelHelper.FormFeed(this.txtIPAddress.Text);

        }
    }
}
