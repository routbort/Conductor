using System;
using System.Drawing;
using System.Windows.Forms;

namespace Conductor.GUI
{
    public class TouchButton : System.Windows.Forms.Button
    {
        StringFormat _Format = new StringFormat();
        string _Text = "";

        public TouchButton()
        {
            this.Resize += new System.EventHandler(this.TouchButton_Resize);       
            base.Text = "";
            _Format.LineAlignment = StringAlignment.Center;
            _Format.Alignment = StringAlignment.Center;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawString(_Text, this.Font, Brushes.Black, ClientRectangle, _Format);
        }

        public new string Text
        {
            get { return _Text; }
            set
            {
                _Text = value;
                ResizeButton();
            }
        }

        void ResizeButton()
        {
            if (_Text=="")
                return;
            Font f = FontHelper.GetFittingFont(this, this.ClientRectangle, _Text,_Format, base.Font.FontFamily.ToString());
            this.Font = f;
        }

        void TouchButton_Resize(object sender, EventArgs e)
        {
            ResizeButton();
        }
    }
}
