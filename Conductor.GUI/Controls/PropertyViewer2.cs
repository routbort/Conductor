using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace Conductor.GUI
{
    public partial class PropertyViewer2 : UserControl
    {
        #region Public Constructors

        public PropertyViewer2()
        {
            InitializeComponent();
            this.dataGrid1.Grid.ColumnHeadersVisible = false;
            this.dataGrid1.Grid.ReadOnly = true;
            this.dataGrid1.Grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGrid1.Grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGrid1.CaptionLabelVisible = true;
            this.dataGrid1.AllowColumnChooser = false;
            this.dataGrid1.CaptionLabel.Text = "Selected properties";
        }

        #endregion Public Constructors

        #region Public Properties

        public List<string> BoundProperties
        { get { return _BoundProperties; } }

        public object SelectedObject
        {
            get { return _SelectedObject; }
            set
            {
                _SelectedObject = value;
                BindProperties();
            }
        }

        #endregion Public Properties

        #region Public Methods

        public void RefreshBoundObject()
        {
            BindProperties();
        }

        #endregion Public Methods

        #region Private Fields

        private List<string> _BoundProperties = new List<string>();
        private object _SelectedObject = null;

        #endregion Private Fields

        #region Private Methods

        private void BindProperties()
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
            else
                this.dataGrid1.Grid.DataSource = null;
        }

        #endregion Private Methods
    }
}