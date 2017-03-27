namespace Conductor.Devices.PerceptionRackScanner
{
    partial class SimpleRackScanControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rackScanLogViewer1 = new Conductor.Devices.PerceptionRackScanner.RackScanProgressControl();
            this.SuspendLayout();
            // 
            // rackScanLogViewer1
            // 
            this.rackScanLogViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rackScanLogViewer1.Location = new System.Drawing.Point(0, 0);
            this.rackScanLogViewer1.Name = "rackScanLogViewer1";
            this.rackScanLogViewer1.ShowProgress = true;
            this.rackScanLogViewer1.Size = new System.Drawing.Size(150, 150);
            this.rackScanLogViewer1.TabIndex = 0;
            // 
            // SimpleRackScanControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rackScanLogViewer1);
            this.Name = "SimpleRackScanControl";
            this.ResumeLayout(false);

        }

        #endregion

        private RackScanProgressControl rackScanLogViewer1;
    }
}
