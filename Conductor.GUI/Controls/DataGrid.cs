﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace Conductor.GUI
{
    public partial class DataGrid : UserControl
    {
        public DataGrid()
        {
            InitializeComponent();
            this.AllowColumnChooser = true;
            this.CaptionLabelVisible = true;
            this.Grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Grid.AllowUserToAddRows = false;
            this.Grid.AllowUserToDeleteRows = false;
            this.Grid.ClearSelection();
            this.Grid.AllowUserToResizeRows = false;
            this.Grid.DisplayOnlyMode = false;
            this.Grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            SetCaptionVisibilites();
        }

        #region Property proxies

        public int SelectedRowIndex
        {
            get { return dgv.SelectedRowIndex; }
            set
            {
                dgv.SelectedRowIndex = value;
            }
        }

        public object DataSource
        {
            get { return this.Grid.DataSource; }
            set { this.Grid.DataSource = value; }
        }

        #endregion

        void pbChooser_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Right)
                this.dgv.ShowColumnChooser(this.PointToScreen(me.Location));
        }

        public DataGridBase Grid { get { return this.dgv; } }

        public Label CaptionLabel { get { return this.lblCaption; } }

        void SetCaptionVisibilites()
        {

            System.Diagnostics.Debug.WriteLine("Setting caption icon visibility");
            this.pbChooser.Visible = (this.CaptionLabelVisible && AllowColumnChooser);
            this.cmdBigger.Visible = (this.CaptionLabelVisible && AllowScaleButtons);
            this.cmdSmaller.Visible = (this.CaptionLabelVisible && AllowScaleButtons);
        }

        bool _CaptionLabelVisible = true;
        public bool CaptionLabelVisible
        {
            get { return _CaptionLabelVisible; }
            set
            {
                _CaptionLabelVisible = value;
                this.lblCaption.Visible = value;
                SetCaptionVisibilites();
            }
        }

        bool _AllowScaleButtons = true;
        public bool AllowScaleButtons

        {
            get
            {
                return _AllowScaleButtons;
            }

            set
            {
                _AllowScaleButtons = value;
                SetCaptionVisibilites();
            }
        }

        bool _AllowColumnChooser = true;
        public bool AllowColumnChooser
        {
            get
            {
                return _AllowColumnChooser;
            }
            set
            {
                _AllowColumnChooser = value;
                SetCaptionVisibilites();
            }

        }

        private void cmdBigger_Click(object sender, EventArgs e)
        {
            AdjustFont(2f);
        }

        private void cmdSmaller_Click(object sender, EventArgs e)
        {
            AdjustFont(-1f);
        }

        void AdjustFont(float amount)
        {
            Font currentFont = this.Grid.DefaultCellStyle.Font;
            float currentSize = currentFont.Size;
            currentSize += amount;
            this.Grid.DefaultCellStyle.Font = new System.Drawing.Font(currentFont.Name, currentSize, currentFont.Style, currentFont.Unit);



        }

        private void WorkflowDataGrid_Load(object sender, EventArgs e)
        {
            this.pbChooser.Visible = true;
            this.cmdSmaller.Visible = true;
            this.cmdBigger.Visible = true;
            SetCaptionVisibilites();
        }

        private void dgv_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void dgv_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}
