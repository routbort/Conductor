namespace Conductor.Configuration
{
    partial class SettingsControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.lblEntityName = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.cmdCreateNewEntity = new System.Windows.Forms.Button();
            this.txtNewEntityName = new System.Windows.Forms.TextBox();
            this.cmdSave = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmbEntityTypes = new  CancellableComboBox();
            this.cmbEntityNames = new CancellableComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Setting Type:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblEntityName
            // 
            this.lblEntityName.AutoSize = true;
            this.lblEntityName.Location = new System.Drawing.Point(18, 43);
            this.lblEntityName.Name = "lblEntityName";
            this.lblEntityName.Size = new System.Drawing.Size(66, 13);
            this.lblEntityName.TabIndex = 3;
            this.lblEntityName.Text = "Settings For:";
            this.lblEntityName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new System.Drawing.Point(1, 75);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(632, 443);
            this.dataGridView1.TabIndex = 9;
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            // 
            // cmdCreateNewEntity
            // 
            this.cmdCreateNewEntity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCreateNewEntity.Enabled = false;
            this.cmdCreateNewEntity.Location = new System.Drawing.Point(387, 38);
            this.cmdCreateNewEntity.Name = "cmdCreateNewEntity";
            this.cmdCreateNewEntity.Size = new System.Drawing.Size(232, 23);
            this.cmdCreateNewEntity.TabIndex = 5;
            this.cmdCreateNewEntity.Text = "Create ...";
            this.cmdCreateNewEntity.UseVisualStyleBackColor = true;
            this.cmdCreateNewEntity.Click += new System.EventHandler(this.cmdCreateNewEntity_Click);
            // 
            // txtNewEntityName
            // 
            this.txtNewEntityName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewEntityName.Location = new System.Drawing.Point(387, 13);
            this.txtNewEntityName.Name = "txtNewEntityName";
            this.txtNewEntityName.Size = new System.Drawing.Size(232, 20);
            this.txtNewEntityName.TabIndex = 6;
            this.txtNewEntityName.TextChanged += new System.EventHandler(this.txtNewEntityName_TextChanged);
            // 
            // cmdSave
            // 
            this.cmdSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdSave.Enabled = false;
            this.cmdSave.Location = new System.Drawing.Point(8, 524);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(105, 23);
            this.cmdSave.TabIndex = 7;
            this.cmdSave.Text = "Save changes";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdCancel.Enabled = false;
            this.cmdCancel.Location = new System.Drawing.Point(119, 524);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(96, 23);
            this.cmdCancel.TabIndex = 8;
            this.cmdCancel.Text = "Cancel changes";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmbEntityTypes
            // 
            this.cmbEntityTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEntityTypes.FormattingEnabled = true;
            this.cmbEntityTypes.Location = new System.Drawing.Point(95, 12);
            this.cmbEntityTypes.Name = "cmbEntityTypes";
            this.cmbEntityTypes.Size = new System.Drawing.Size(223, 21);
            this.cmbEntityTypes.TabIndex = 0;
            this.cmbEntityTypes.BeforeUpdate += new System.ComponentModel.CancelEventHandler(this.cmbEntityTypes_BeforeUpdate);
            this.cmbEntityTypes.SelectedValueChanged += new System.EventHandler(this.cmbEntityTypes_SelectedValueChanged);
            // 
            // cmbEntityNames
            // 
            this.cmbEntityNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEntityNames.FormattingEnabled = true;
            this.cmbEntityNames.Location = new System.Drawing.Point(95, 39);
            this.cmbEntityNames.Name = "cmbEntityNames";
            this.cmbEntityNames.Size = new System.Drawing.Size(223, 21);
            this.cmbEntityNames.TabIndex = 1;
            this.cmbEntityNames.BeforeUpdate += new System.ComponentModel.CancelEventHandler(this.cmbEntityNames_BeforeUpdate);
            this.cmbEntityNames.SelectedValueChanged += new System.EventHandler(this.cmbEntityNames_SelectedValueChanged);
            // 
            // SettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.txtNewEntityName);
            this.Controls.Add(this.cmdCreateNewEntity);
            this.Controls.Add(this.lblEntityName);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbEntityTypes);
            this.Controls.Add(this.cmbEntityNames);
            this.Name = "SettingsControl";
            this.Size = new System.Drawing.Size(635, 559);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CancellableComboBox cmbEntityTypes;
        private CancellableComboBox cmbEntityNames;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblEntityName;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button cmdCreateNewEntity;
        private System.Windows.Forms.TextBox txtNewEntityName;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.Button cmdCancel;
    }
}
