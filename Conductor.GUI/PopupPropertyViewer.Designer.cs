namespace Conductor.GUI
{
    partial class PopupPropertyViewer
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.propertyViewer1 = new Conductor.GUI.PropertyViewer();
            this.SuspendLayout();
            // 
            // propertyViewer1
            // 
            this.propertyViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyViewer1.Location = new System.Drawing.Point(0, 0);
            this.propertyViewer1.Name = "propertyViewer1";
            this.propertyViewer1.SelectedObject = null;
            this.propertyViewer1.Size = new System.Drawing.Size(300, 296);
            this.propertyViewer1.TabIndex = 0;
            // 
            // PopupPropertyViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 296);
            this.ControlBox = false;
            this.Controls.Add(this.propertyViewer1);
            this.Name = "PopupPropertyViewer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.ResumeLayout(false);

        }

        #endregion

        private PropertyViewer propertyViewer1;
    }
}