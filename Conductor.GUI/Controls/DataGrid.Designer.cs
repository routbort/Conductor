namespace Conductor.GUI
{
    partial class DataGrid
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataGrid));
            this.lblCaption = new System.Windows.Forms.Label();
            this.pbChooser = new System.Windows.Forms.PictureBox();
            this.cmdBigger = new System.Windows.Forms.Button();
            this.cmdSmaller = new System.Windows.Forms.Button();
            this.dgv = new Conductor.GUI.DataGridBase();
            ((System.ComponentModel.ISupportInitialize)(this.pbChooser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCaption
            // 
            this.lblCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCaption.Location = new System.Drawing.Point(0, 0);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(204, 24);
            this.lblCaption.TabIndex = 0;
            this.lblCaption.Text = "DataGrid";
            this.lblCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbChooser
            // 
            this.pbChooser.Image = ((System.Drawing.Image)(resources.GetObject("pbChooser.Image")));
            this.pbChooser.Location = new System.Drawing.Point(4, 4);
            this.pbChooser.Name = "pbChooser";
            this.pbChooser.Size = new System.Drawing.Size(16, 16);
            this.pbChooser.TabIndex = 2;
            this.pbChooser.TabStop = false;
            this.pbChooser.Click += new System.EventHandler(this.pbChooser_Click);
            // 
            // cmdBigger
            // 
            this.cmdBigger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdBigger.Location = new System.Drawing.Point(179, 0);
            this.cmdBigger.Name = "cmdBigger";
            this.cmdBigger.Size = new System.Drawing.Size(19, 23);
            this.cmdBigger.TabIndex = 3;
            this.cmdBigger.Text = "+";
            this.cmdBigger.UseVisualStyleBackColor = true;
            this.cmdBigger.Click += new System.EventHandler(this.cmdBigger_Click);
            // 
            // cmdSmaller
            // 
            this.cmdSmaller.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSmaller.Location = new System.Drawing.Point(158, 0);
            this.cmdSmaller.Name = "cmdSmaller";
            this.cmdSmaller.Size = new System.Drawing.Size(19, 23);
            this.cmdSmaller.TabIndex = 4;
            this.cmdSmaller.Text = "-";
            this.cmdSmaller.UseVisualStyleBackColor = true;
            this.cmdSmaller.Click += new System.EventHandler(this.cmdSmaller_Click);
            // 
            // dgv
            // 
            this.dgv.ColumnChooserMaxHeight = 300;
            this.dgv.ColumnChooserWidth = 200;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.DisplayOnlyMode = false;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.EnableVisibleColumnList = true;
            this.dgv.Location = new System.Drawing.Point(0, 24);
            this.dgv.Name = "dgv";
            this.dgv.RowHeadersVisible = false;
            this.dgv.SelectedRowIndex = -1;
            this.dgv.Size = new System.Drawing.Size(204, 210);
            this.dgv.Sortable = false;
            this.dgv.TabIndex = 1;
            this.dgv.VisibleColumnList = "";
            this.dgv.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgv_KeyDown);
            this.dgv.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgv_KeyPress);
            // 
            // DataGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pbChooser);
            this.Controls.Add(this.cmdBigger);
            this.Controls.Add(this.cmdSmaller);
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.lblCaption);
            this.Name = "DataGrid";
            this.Size = new System.Drawing.Size(204, 234);
            this.Load += new System.EventHandler(this.WorkflowDataGrid_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbChooser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblCaption;
        private DataGridBase dgv;
        private System.Windows.Forms.PictureBox pbChooser;
        private System.Windows.Forms.Button cmdBigger;
        private System.Windows.Forms.Button cmdSmaller;
    }
}
