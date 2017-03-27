using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Conductor.GUI
{
    public partial class PropertyViewer : UserControl
    {
        public PropertyViewer()
        {
            InitializeComponent();
            this.dataGrid1.Grid.ColumnHeadersVisible = false;
            this.dataGrid1.Grid.ReadOnly = true;
            this.dataGrid1.Grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill ;
            this.dataGrid1.Grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;  
            this.pg.ToolbarVisible = false;
            this.dataGrid1.CaptionLabelVisible = true;
            this.dataGrid1.AllowColumnChooser = false;
            this.dataGrid1.CaptionLabel.Text = "Selected properties";
        }


        List<string> _BoundProperties = new List<string>();
        public List<string> BoundProperties { get { return _BoundProperties; } }


        object _SelectedObject = null;
        public object SelectedObject
        {
            get { return _SelectedObject; }
            set
            {
                _SelectedObject = value;
                this.pg.SelectedObject = value;
                BindProperties();
            }

        }

        void BindProperties()
        {

            List<KeyValuePair<string, string>> output = new List<KeyValuePair<string, string>>();
            if (_SelectedObject != null)
            {
                BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
                if (_BoundProperties.Count == 0)
                    foreach (PropertyInfo property in _SelectedObject.GetType().GetProperties(flags))
                        output.Add(new KeyValuePair<string, string>(property.Name, (property.GetValue(_SelectedObject, null) == null) ? "" : property.GetValue(_SelectedObject, null).ToString()));
                else
                {
                    foreach (string propertyName in _BoundProperties)
                    {
                        PropertyInfo property = _SelectedObject.GetType().GetProperty(propertyName);
                        if (property != null)
                            output.Add(new KeyValuePair<string, string>(property.Name, (property.GetValue(_SelectedObject, null) == null) ? "" : property.GetValue(_SelectedObject, null).ToString()));
                    }
                }
                this.dataGrid1.Grid.DataSource = output;
            }
         
        }


    }
}
