namespace Conductor.Devices.CABPrinter
{
    partial class frmTest
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
            this.txtTemplate = new System.Windows.Forms.TextBox();
            this.txtIPAddress = new System.Windows.Forms.TextBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.chkFormFeed = new System.Windows.Forms.CheckBox();
            this.upCopyNumber = new System.Windows.Forms.NumericUpDown();
            this.cmdPrint = new System.Windows.Forms.Button();
            this.udX = new System.Windows.Forms.NumericUpDown();
            this.udY = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.upCopyNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udY)).BeginInit();
            this.SuspendLayout();
            // 
            // txtTemplate
            // 
            this.txtTemplate.Location = new System.Drawing.Point(12, 12);
            this.txtTemplate.Multiline = true;
            this.txtTemplate.Name = "txtTemplate";
            this.txtTemplate.Size = new System.Drawing.Size(432, 421);
            this.txtTemplate.TabIndex = 0;
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.Location = new System.Drawing.Point(533, 325);
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.Size = new System.Drawing.Size(100, 20);
            this.txtIPAddress.TabIndex = 1;
            this.txtIPAddress.Text = "10.127.68.220";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Location = new System.Drawing.Point(462, 27);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(216, 226);
            this.propertyGrid1.TabIndex = 2;
            // 
            // chkFormFeed
            // 
            this.chkFormFeed.AutoSize = true;
            this.chkFormFeed.Location = new System.Drawing.Point(504, 380);
            this.chkFormFeed.Name = "chkFormFeed";
            this.chkFormFeed.Size = new System.Drawing.Size(103, 17);
            this.chkFormFeed.TabIndex = 3;
            this.chkFormFeed.Text = "Form feed after?";
            this.chkFormFeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkFormFeed.UseVisualStyleBackColor = true;
            // 
            // upCopyNumber
            // 
            this.upCopyNumber.Location = new System.Drawing.Point(533, 351);
            this.upCopyNumber.Name = "upCopyNumber";
            this.upCopyNumber.Size = new System.Drawing.Size(100, 20);
            this.upCopyNumber.TabIndex = 4;
            this.upCopyNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cmdPrint
            // 
            this.cmdPrint.Location = new System.Drawing.Point(492, 404);
            this.cmdPrint.Name = "cmdPrint";
            this.cmdPrint.Size = new System.Drawing.Size(108, 33);
            this.cmdPrint.TabIndex = 5;
            this.cmdPrint.Text = "Print";
            this.cmdPrint.UseVisualStyleBackColor = true;
            this.cmdPrint.Click += new System.EventHandler(this.cmdPrint_Click);
            // 
            // udX
            // 
            this.udX.Location = new System.Drawing.Point(533, 272);
            this.udX.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.udX.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this.udX.Name = "udX";
            this.udX.Size = new System.Drawing.Size(100, 20);
            this.udX.TabIndex = 6;
            // 
            // udY
            // 
            this.udY.Location = new System.Drawing.Point(533, 298);
            this.udY.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.udY.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this.udY.Name = "udY";
            this.udY.Size = new System.Drawing.Size(100, 20);
            this.udY.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(469, 274);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "X offset";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(469, 300);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Y offset";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(469, 328);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "IP address";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(469, 353);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Copies";
            // 
            // frmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 460);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.udY);
            this.Controls.Add(this.udX);
            this.Controls.Add(this.cmdPrint);
            this.Controls.Add(this.upCopyNumber);
            this.Controls.Add(this.chkFormFeed);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.txtIPAddress);
            this.Controls.Add(this.txtTemplate);
            this.Name = "frmTest";
            this.Text = "CAB Printer Tester";
            ((System.ComponentModel.ISupportInitialize)(this.upCopyNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udY)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtTemplate;
        private System.Windows.Forms.TextBox txtIPAddress;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.CheckBox chkFormFeed;
        private System.Windows.Forms.NumericUpDown upCopyNumber;
        private System.Windows.Forms.Button cmdPrint;
        private System.Windows.Forms.NumericUpDown udX;
        private System.Windows.Forms.NumericUpDown udY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

