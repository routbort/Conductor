namespace Conductor.GUI
{
    partial class TouchNumberInputForm
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
            this.touchNumberInput1 = new Conductor.GUI.TouchNumberInput();
            this.SuspendLayout();
            // 
            // touchNumberInput1
            // 
            this.touchNumberInput1.Location = new System.Drawing.Point(0, 0);
            this.touchNumberInput1.Name = "touchNumberInput1";
            this.touchNumberInput1.Size = new System.Drawing.Size(296, 451);
            this.touchNumberInput1.TabIndex = 0;
            // 
            // TouchNumberInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 453);
            this.Controls.Add(this.touchNumberInput1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TouchNumberInputForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enter a number";
            this.ResumeLayout(false);

        }

        #endregion

        private TouchNumberInput touchNumberInput1;
    }
}