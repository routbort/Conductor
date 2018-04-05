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
    public partial class TouchButtonContainer : UserControl
    {
        public TouchButtonContainer()
        {
            InitializeComponent();
            this.pnlControls.Resize += pnlControls_Resize;
        }

        private void pnlControls_Resize(object sender, EventArgs e)
        {
            ResizeButtonren();
        }


        List<Control> _Controls = new List<Control>();

        const int MIN_BUTTON_WIDTH = 10;

        public void AddControl(Control uc)
        {

            _Controls.Add(uc);
            this.pnlControls.Controls.Add(uc);
            ResizeButtonren();
        }

        public int VerticalPadding { get; set; } = 10;
        public int HorizontalPadding { get; set; } = 10;
        public int MinButtonHeight { get; set; } = 10;
        public int MaxButtonHeight { get; set; } = 50;

        void ResizeButtonren()

        {
            this.SuspendLayout();
            if (_Controls.Count == 0) return;
            int n = _Controls.Count;
            int minContainerHeight = _Controls.Count * (MinButtonHeight + VerticalPadding) + VerticalPadding;
            int availableTotalHeight = this.Height;
            int buttonHeight = Math.Min(MaxButtonHeight, Math.Max(MinButtonHeight, (availableTotalHeight - (n + 1) * VerticalPadding) / n));
            int buttonWidth = Math.Max(this.Width - 2 * HorizontalPadding, MIN_BUTTON_WIDTH);

            int index = 0;
            foreach (Control uc in _Controls)
            {
                uc.Width = buttonWidth;
                uc.Left = HorizontalPadding;
                uc.Height = buttonHeight;
                uc.Top = VerticalPadding + (index * (buttonHeight + VerticalPadding));
                index++;
            }

            if (this.pnlControls.VerticalScroll.Visible)
            {
                buttonWidth = Math.Max(this.Width - System.Windows.Forms.SystemInformation.VerticalScrollBarWidth - 2 * HorizontalPadding, MIN_BUTTON_WIDTH);
                foreach (Control uc in _Controls)
                    uc.Width = buttonWidth;
            }

            this.ResumeLayout();
        }



    }
}
