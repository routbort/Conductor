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
    public partial class PopupPropertyViewer : Form
    {
        public PopupPropertyViewer()
        {
            InitializeComponent();
            tmrWindowCheck = new System.Timers.Timer();
            tmrWindowCheck.Elapsed += TmrWindowCheck_Elapsed;
            tmrWindowCheck.Interval = 200;

        }

        public PropertyViewer PropertyViewer { get { return this.propertyViewer1; } }

        private void TmrWindowCheck_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Invoke((MethodInvoker)
            delegate
            {
                if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
                    return;  //could be resizing window
            Point pos = Control.MousePosition;
            bool inForm = pos.X >= Left && pos.Y >= Top && pos.X < Right && pos.Y < Bottom;
            if (!inForm)
            {
                tmrWindowCheck.Stop();
                this.Hide();
            }
        });
        }

    System.Timers.Timer tmrWindowCheck = null;
    public object SelectedObject
    {
        get { return this.propertyViewer1.SelectedObject; }
        set { this.propertyViewer1.SelectedObject = value; }
    }

    protected override bool ShowWithoutActivation
    {
        get { return true; }
    }

    public new void Show()
    {
        tmrWindowCheck.Stop();
        tmrWindowCheck.Start();
        base.Show();
    }

    public new void ShowDialog()
    {
        tmrWindowCheck.Stop();
        tmrWindowCheck.Start();
        base.ShowDialog();

    }


}
}
