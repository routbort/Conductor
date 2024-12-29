namespace Conductor.GUI
{
    partial class PropertyViewer2
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
            this.dataGrid1 = new Conductor.GUI.DataGrid();
            this.SuspendLayout();
            // 
            // dataGrid1
            // 
            this.dataGrid1.AllowColumnChooser = false;
            this.dataGrid1.AllowScaleButtons = true;
            this.dataGrid1.AutoSelectFirstRow = false;
            this.dataGrid1.CaptionLabelVisible = false;
            this.dataGrid1.DataSource = null;
            this.dataGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGrid1.Location = new System.Drawing.Point(0, 0);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.RowHeightPadding = 4;
            this.dataGrid1.SelectedRowIndex = -1;
            this.dataGrid1.Size = new System.Drawing.Size(391, 533);
            this.dataGrid1.TabIndex = 0;
            // 
            // PropertyViewer2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGrid1);
            this.Name = "PropertyViewer2";
            this.Size = new System.Drawing.Size(391, 533);
            this.ResumeLayout(false);

        }

        #endregion
        private DataGrid dataGrid1;
    }
}
