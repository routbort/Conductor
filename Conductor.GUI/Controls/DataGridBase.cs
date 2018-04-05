using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Conductor.GUI
{

    public class DataGridBase : DataGridView

    {  

 
        public bool AutoSelectFirstRow { get; set; }

        private void CustomDataGridView_SelectionChanged(object sender, EventArgs e)
        {
         // if (!_InternalSelectionInProgress)
                ClearSelection();
        }


        public delegate void SelectedRowChangedEventHandler(object sender, int SelectedRowIndex);

        [Browsable(true)]
        public event SelectedRowChangedEventHandler SelectedRowChanged;

        public int SelectedRowIndex


        {
            get { return _SelectedRowIndex; }

            set
            {

                if (value < -1 || value >= this.Rows.Count) throw new ApplicationException("Invalid row index specified");
                int oldValue = _SelectedRowIndex;
                //   if (value == -1  || value != _SelectedRowIndex)
                if (true)
                {
                  
                    ClearHighlighting();
                    _SelectedRowIndex = value;
                    if (value != -1)
                    {
                        this.Rows[value].DefaultCellStyle.ForeColor = this.DefaultCellStyle.SelectionForeColor; // System.Drawing.SystemColors.HighlightText;
                        this.Rows[value].DefaultCellStyle.BackColor = this.DefaultCellStyle.SelectionBackColor;// System.Drawing.SystemColors.Highlight;
                          this.CurrentCell = this.Rows[value].Cells[0];
                      }
                    else
                    {
                        //   _InternalSelectionInProgress = true;
                        this.CurrentCell = null;
                        //  _InternalSelectionInProgress = false;
                    }
                   if ( _SelectedRowIndex!= oldValue || _SelectedRowIndex ==-1)
                        SelectedRowChanged?.Invoke(this, value);
                }
            }
        }

        public object SelectedRowObject
        {
            get
            {
                if (_SelectedRowIndex == -1)
                    return null;
                return this.Rows[_SelectedRowIndex].DataBoundItem;
            }
        }


        int _SelectedRowIndex = -1;

        void UnhighlightRow(int rowIndex)
        {
            this.Rows[rowIndex].DefaultCellStyle.ForeColor = this.DefaultCellStyle.ForeColor;
            this.Rows[rowIndex].DefaultCellStyle.BackColor = this.DefaultCellStyle.BackColor;
        }

        void ClearHighlighting()
        {
            for (int i = 0; i < this.Rows.Count; i++)
                UnhighlightRow(i);
        }

        CheckedListBox _ColumnChooserCheckedListBox;
        ToolStripDropDown _ColumnChooserWindow;

        public int ColumnChooserMaxHeight { get; set; } = 300;
        public int ColumnChooserWidth { get; set; } = 200;

        public DataGridBase() : base()
        {
            this.RowHeadersVisible = false;
            _ColumnChooserCheckedListBox = new CheckedListBox();
            _ColumnChooserCheckedListBox.CheckOnClick = true;
            _ColumnChooserCheckedListBox.ItemCheck += new ItemCheckEventHandler(ColumnChooserItemChecked);
            ToolStripControlHost controlHost = new ToolStripControlHost(_ColumnChooserCheckedListBox);
            controlHost.Padding = Padding.Empty;
            controlHost.Margin = Padding.Empty;
            controlHost.AutoSize = false;
            _ColumnChooserWindow = new ToolStripDropDown();
            _ColumnChooserWindow.Padding = Padding.Empty;
            _ColumnChooserWindow.Items.Add(controlHost);
            this.SelectionChanged += CustomDataGridView_SelectionChanged;
            this.CellClick += CustomDataGridView_CellClick;
           
            this.DataSourceChanged += CustomDataGridView_DataSourceChanged;
         
        }



        private void CustomDataGridView_DataSourceChanged(object sender, EventArgs e)
        {

            if (AutoSelectFirstRow && this.Rows.Count > 0)
                SelectedRowIndex = 0;
            else
                SelectedRowIndex = -1;

            ResetColumnVisibility();
        }




        private void CustomDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (!_DisplayOnlyMode)
                SelectedRowIndex = e.RowIndex;
        }

        public void ShowColumnChooser(Point p)
        {
            _ColumnChooserCheckedListBox.Items.Clear();
            foreach (DataGridViewColumn c in this.Columns)
                _ColumnChooserCheckedListBox.Items.Add(c.HeaderText, c.Visible);
            int PreferredHeight = (_ColumnChooserCheckedListBox.Items.Count * 16) + 7;
            _ColumnChooserCheckedListBox.Height = (PreferredHeight < this.ColumnChooserMaxHeight) ? PreferredHeight : this.ColumnChooserMaxHeight;
            _ColumnChooserCheckedListBox.Width = this.ColumnChooserWidth;
            _ColumnChooserWindow.Show(p);
        }

        void ColumnChooserItemChecked(object sender, ItemCheckEventArgs e)
        {
            this.Columns[e.Index].Visible = (e.NewValue == CheckState.Checked);
        }

        bool _DisplayOnlyMode = false;
        public bool DisplayOnlyMode
        {
            get { return _DisplayOnlyMode; }

            set
            {
                if (_DisplayOnlyMode)
                {
                    ClearSelection();
                    ClearHighlighting();
                    this.ReadOnly = true;
                }
                _DisplayOnlyMode = value;
                UpdateSortability();
            }
        }

        bool _Sortable;
        public bool Sortable
        {
            get
            {
                return _Sortable;
            }
            set
            {
                _Sortable = value;
                UpdateSortability();
            }

        }

        private void UpdateSortability()
        {
            foreach (DataGridViewColumn col in Columns)
                if (_Sortable && !_DisplayOnlyMode)
                    col.SortMode = DataGridViewColumnSortMode.Automatic;
                else
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;

        }

        private void ResetColumnVisibility()
        {
            List<string> visible = new List<string>();
            if (_VisibleColumnList != null && _VisibleColumnList != "")
                visible = _VisibleColumnList.Split(new char[] { ';', ',' }).ToList<string>();
            foreach (DataGridViewColumn col in this.Columns)
                col.Visible = (visible.Count == 0 || visible.Contains(col.Name));
        }

        string GetVisibleColumns()
        {
            string output = "";
            foreach (DataGridViewColumn column in Columns)
                if (column.Visible)
                    output += column.Name + ",";
            return output.TrimEnd(new char[] { ';' });
        }

        string _VisibleColumnList = "";
        public string VisibleColumnList
        {
            set
            {
                _VisibleColumnList = value;
                ResetColumnVisibility();
            }

            get
            {
                string output = "";
                bool AllVisible = true;
                foreach (DataGridViewColumn column in this.Columns)
                {
                    if (column.Visible)
                        output += column.Name + ";";
                    else
                        AllVisible = false;
                }
                if (AllVisible) return "";
                return output;
            }
        }

        bool _EnableVisibleColumnList = true;
        public bool EnableVisibleColumnList
        {
            get { return _EnableVisibleColumnList; }

            set
            {
                _EnableVisibleColumnList = value;
                ResetColumnVisibility();
            }
        }




        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {


        }

    }
}
