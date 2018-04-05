using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Conductor.GUI
{
    public partial class TouchNumberInput : UserControl
    {

        public event EventHandler OK_Clicked;
        public event EventHandler Cancel_Clicked;

        [DllImport("user32.dll")]
        private static extern int HideCaret(IntPtr hwnd);

        public TouchNumberInput()
        {
            InitializeComponent();

            foreach (var c in this.Controls)
            {
                Button b = (c as Button);
                if (b != null)
                    b.Click += B_Click;

            }
            this.txtNumber.KeyDown += TxtNumber_KeyDown;
            this.txtNumber.Enter += CleanTextboxAppearance;
            this.txtNumber.GotFocus += CleanTextboxAppearance;
        }

  

        void CleanTextboxAppearance (object sender, EventArgs e)
        {

            this.txtNumber.SelectionStart = Int32.MaxValue;
            HideCaret(this.txtNumber.Handle);

        }
        void DeleteChar()
        {
            if (this.txtNumber.Text.Length > 0)
                this.txtNumber.Text = this.txtNumber.Text.Remove(this.txtNumber.Text.Length - 1);
        }


        private void TxtNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                OK_Clicked?.Invoke(this, EventArgs.Empty);
            }

            if (e.KeyCode  == Keys.Delete || e.KeyCode == Keys.Back)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                DeleteChar();

            }
        }

        private void B_Click(object sender, EventArgs e)
        {

            this.txtNumber.Focus();
            this.txtNumber.SelectionStart = Int32.MaxValue;
 
            Button b = sender as Button;

            switch (b.Name)
            {
                case "OK":
                    OK_Clicked?.Invoke(this, EventArgs.Empty);
                    break;

                case "Cancel":
                    Cancel_Clicked?.Invoke(this, EventArgs.Empty);
                    break;

                case "Del":
                    DeleteChar();
                    break;

                default:
                    this.txtNumber.Text += b.Name.Replace("b", "");
                    break;
            }

        }


      //  private int? _Value;

        public int? Value
        {
            get
            {
                int value;
                if (Int32.TryParse(this.txtNumber.Text, out value))
                    return value;
                return null;
            }

        }




    }
}
