using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Conductor.GUI
{
    public partial class TouchNumberInputForm : Form
    {
        public TouchNumberInputForm()
        {
            InitializeComponent();
            this.touchNumberInput1.OK_Clicked += TouchNumberInput1_OK_Clicked;
            this.touchNumberInput1.Cancel_Clicked += TouchNumberInput1_Cancel_Clicked;
        }

        int? _Value = null;

        public int? Value { get { return _Value; } }


        private void TouchNumberInput1_Cancel_Clicked
            (object sender, EventArgs e)
        {
            _Value = null;
            this.Close();

        }

        private void TouchNumberInput1_OK_Clicked
            (object sender, EventArgs e)
        {
            _Value = this.touchNumberInput1.Value;
            this.Close();
        }
    }
}
