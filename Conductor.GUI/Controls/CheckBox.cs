using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Conductor.GUI
{

    /// <summary>
    /// Basically a simple CheckBox control with the one added feature of allowing a specifized size for the CheckBox portion
    /// </summary>
    public partial class CheckBox : UserControl
    {
        public event EventHandler CheckedChanged;

        public CheckBox()
        {
            InitializeComponent();
        }

        private int _CheckSize = 50;

        public int CheckSize
        {
            get { return _CheckSize; }
            set
            {
                _CheckSize = value;
                Redraw();
            }
        }

        public bool AutoCheck { get; set; }


        private string _Text;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        override public string Text
        {
            get { return _Text; }
            set { _Text = value; Redraw(); }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            DrawControl(pe.Graphics);
        }

        void Redraw()
        {
            this.Invalidate();

        }

        private ContentAlignment _TextAlign = ContentAlignment.MiddleCenter;

        public ContentAlignment TextAlign
        {

            get { return _TextAlign; }
            set
            {
                _TextAlign = value;
                Redraw();
            }
        }


        private bool _Checked;

        public bool Checked
        {
            get { return _Checked; }
            set
            {
                if (value == Checked) return;
                _Checked = value;
                Redraw();
                CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        int _Padding = 2;

        Rectangle GetCheckboxRectangle()
        {
            Point checkboxOrigin = new Point(_Padding, (this.Height - 2 * _Padding - CheckSize) / 2);
            Rectangle rect = new Rectangle(checkboxOrigin, new Size(CheckSize, CheckSize));
            return rect;
        }

        private void DrawControl(Graphics graphics)
        {
            Rectangle rect = GetCheckboxRectangle();
            ControlPaint.DrawCheckBox(graphics, rect, this.Checked ? ButtonState.Checked : ButtonState.Normal);
            int originCaption = _Padding * 2 + CheckSize;
            Rectangle textRectangle = new Rectangle(originCaption, _Padding, this.Width - originCaption - _Padding * 3, this.Height - _Padding * 2);
            Brush fontBrush = new SolidBrush(this.ForeColor);
            StringFormat format = new StringFormat();
            switch (this.TextAlign)
            {
                case ContentAlignment.TopRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomRight:
                    format.Alignment = StringAlignment.Far;
                    format.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomLeft:
                    format.Alignment = StringAlignment.Near;
                    format.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomCenter:
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Far;
                    break;
            }
            graphics.DrawString(Text, this.Font, fontBrush, textRectangle, format);
            fontBrush.Dispose();
        }

        private void Control_Clicked(object sender, EventArgs e)
        {
            if (AutoCheck)
            {
                this.Checked = !this.Checked;
                return;
            }

            Point p = (e as MouseEventArgs).Location;
            if (GetCheckboxRectangle().Contains(p))
                this.Checked = !this.Checked;
        }

        private void Control_Resized(object sender, EventArgs e)
        {
            Redraw();
        }
    }
}