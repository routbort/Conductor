namespace QuickExample
{
    partial class Form1
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
            Conductor.GUI.CartesianMapper cartesianMapper1 = new Conductor.GUI.CartesianMapper();
            this.cartesianGrid1 = new Conductor.GUI.CartesianGrid();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.cmdSimulateScan = new System.Windows.Forms.Button();
            this.simpleRackScanControl1 = new Conductor.Devices.RackScanner.SimpleRackScanControl();
            this.lblMessage = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbFluidX = new System.Windows.Forms.RadioButton();
            this.rbZiath = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cartesianGrid1
            // 
            this.cartesianGrid1.AllowReordering = true;
            this.cartesianGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cartesianGrid1.BackColor = System.Drawing.SystemColors.Control;
            this.cartesianGrid1.CaptionHeight = 35;
            this.cartesianGrid1.CellHoverDelay = 500;
            this.cartesianGrid1.CellSelectionMode = Conductor.GUI.CartesianGrid.SelectionMode.None;
            this.cartesianGrid1.CutCellsColor = System.Drawing.Color.BurlyWood;
            this.cartesianGrid1.DataBoundItemTypeRestriction = null;
            this.cartesianGrid1.DataSource = null;
            this.cartesianGrid1.Dimensions = new System.Drawing.Size(12, 8);
            this.cartesianGrid1.ErrorCaptionHeight = 35;
            this.cartesianGrid1.FillMode = Conductor.GUI.Mapping.AutoFillMode.Preserve;
            this.cartesianGrid1.ForeColor = System.Drawing.Color.Black;
            this.cartesianGrid1.GridLineWidth = 1;
            this.cartesianGrid1.HideErrors = true;
            this.cartesianGrid1.ItemFillOrder = Conductor.GUI.Mapping.FillOrder.TopBottomLeftRight;
            this.cartesianGrid1.Location = new System.Drawing.Point(4, 122);
            cartesianMapper1.ColCount = 12;
            cartesianMapper1.Order = Conductor.GUI.Mapping.FillOrder.TopBottomLeftRight;
            cartesianMapper1.RowCount = 8;
            this.cartesianGrid1.Mapper = cartesianMapper1;
            this.cartesianGrid1.Name = "cartesianGrid1";
            this.cartesianGrid1.SelectedCellBorderColor = System.Drawing.Color.BlueViolet;
            this.cartesianGrid1.SelectedCellBorderWidth = 4;
            this.cartesianGrid1.ShowObjectPropertyViewerOnHover = true;
            this.cartesianGrid1.Size = new System.Drawing.Size(1016, 607);
            this.cartesianGrid1.TabIndex = 1;
            this.cartesianGrid1.Text = null;
            this.cartesianGrid1.ThrowExceptionsOnErrors = true;
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 42;
            this.listBox1.Items.AddRange(new object[] {
            "Store",
            "Check"});
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(160, 88);
            this.listBox1.TabIndex = 4;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtBarcode);
            this.splitContainer1.Panel1.Controls.Add(this.cmdSimulateScan);
            this.splitContainer1.Panel1.Controls.Add(this.listBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.simpleRackScanControl1);
            this.splitContainer1.Panel2.Controls.Add(this.lblMessage);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(1020, 116);
            this.splitContainer1.SplitterDistance = 160;
            this.splitContainer1.TabIndex = 5;
            // 
            // txtBarcode
            // 
            this.txtBarcode.Location = new System.Drawing.Point(74, 92);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(83, 20);
            this.txtBarcode.TabIndex = 5;
            this.txtBarcode.Text = "1234";
            // 
            // cmdSimulateScan
            // 
            this.cmdSimulateScan.Location = new System.Drawing.Point(4, 90);
            this.cmdSimulateScan.Name = "cmdSimulateScan";
            this.cmdSimulateScan.Size = new System.Drawing.Size(66, 23);
            this.cmdSimulateScan.TabIndex = 4;
            this.cmdSimulateScan.Text = "Sim scan";
            this.cmdSimulateScan.UseVisualStyleBackColor = true;
            this.cmdSimulateScan.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // simpleRackScanControl1
            // 
            this.simpleRackScanControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.simpleRackScanControl1.Location = new System.Drawing.Point(69, 0);
            this.simpleRackScanControl1.Name = "simpleRackScanControl1";
            this.simpleRackScanControl1.Size = new System.Drawing.Size(787, 116);
            this.simpleRackScanControl1.TabIndex = 2;
            this.simpleRackScanControl1.Visible = false;
            // 
            // lblMessage
            // 
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(69, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(787, 116);
            this.lblMessage.TabIndex = 3;
            this.lblMessage.Text = "Message";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbFluidX);
            this.groupBox1.Controls.Add(this.rbZiath);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(69, 116);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Scanner";
            // 
            // rbFluidX
            // 
            this.rbFluidX.AutoSize = true;
            this.rbFluidX.Location = new System.Drawing.Point(6, 70);
            this.rbFluidX.Name = "rbFluidX";
            this.rbFluidX.Size = new System.Drawing.Size(54, 17);
            this.rbFluidX.TabIndex = 1;
            this.rbFluidX.TabStop = true;
            this.rbFluidX.Text = "FluidX";
            this.rbFluidX.UseVisualStyleBackColor = true;
            this.rbFluidX.CheckedChanged += new System.EventHandler(this.rbFluidX_CheckedChanged);
            // 
            // rbZiath
            // 
            this.rbZiath.AutoSize = true;
            this.rbZiath.Location = new System.Drawing.Point(6, 31);
            this.rbZiath.Name = "rbZiath";
            this.rbZiath.Size = new System.Drawing.Size(49, 17);
            this.rbZiath.TabIndex = 0;
            this.rbZiath.TabStop = true;
            this.rbZiath.Text = "Ziath";
            this.rbZiath.UseVisualStyleBackColor = true;
            this.rbZiath.CheckedChanged += new System.EventHandler(this.rbZiath_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 731);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.cartesianGrid1);
            this.Name = "Form1";
            this.Text = "Simple Store & Check Example";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Conductor.GUI.CartesianGrid cartesianGrid1;
        private Conductor.Devices.RackScanner.SimpleRackScanControl simpleRackScanControl1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button cmdSimulateScan;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbFluidX;
        private System.Windows.Forms.RadioButton rbZiath;
    }
}

