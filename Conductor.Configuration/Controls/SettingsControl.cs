using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Conductor.Configuration;

namespace Conductor.Configuration
{
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
        }

        private ConfigBase _config;
        private bool _dirty;
        private string _currentEntityType = null;
        private string _currentEntityName = null;

        public void Bind(ConfigBase config)

        {
            _config = config;
            this.cmbEntityTypes.DataSource = _config.GetEntityTypes();
            SetDirty(false);
        }

        private void cmbEntityTypes_SelectedValueChanged(object sender, EventArgs e)
        {

            BindingList<string> entityNames = _config.GetEntityNames(this.cmbEntityTypes.SelectedValue.ToString());
            if (entityNames.Count == 0)
                entityNames.Add(_config.DefaultEntityName);
            this.cmbEntityNames.DataSource = entityNames;
            this.cmdCreateNewEntity.Text = "Create new " + this.cmbEntityTypes.SelectedValue.ToString();
            _currentEntityType = this.cmbEntityTypes.SelectedValue.ToString();
        }

        private void SetDataGrid()
        {
            if (this.cmbEntityNames.Items.Count == 0)
            {
                dataGridView1.Visible = false;
                return;
            }
            string[] settingNames = _config.GetSettingNames(this.cmbEntityTypes.SelectedValue.ToString());
            this.dataGridView1.DataSource = _config.GetMergedSettings(this.cmbEntityTypes.SelectedValue.ToString(), this.cmbEntityNames.SelectedValue.ToString());
            _currentEntityName = this.cmbEntityNames.SelectedValue.ToString();
        }

        private void cmbEntityNames_SelectedValueChanged(object sender, EventArgs e)
        {
            SetDataGrid();
        }

        private void cmdCreateNewEntity_Click(object sender, EventArgs e)
        {
            if (this.cmbEntityNames.Items.Contains(this.txtNewEntityName.Text))
            {
                MessageBox.Show("This " + this.cmbEntityTypes.SelectedValue.ToString() + " already exists");
                return;
            }
            BindingList<string> choices = this.cmbEntityNames.DataSource as BindingList<string>;
            choices.Add(this.txtNewEntityName.Text);
            this.cmbEntityNames.SelectedIndex = this.cmbEntityNames.FindStringExact(this.txtNewEntityName.Text);
            this.txtNewEntityName.Text = "";
            SetDirty(true);
            Dictionary<string, string> defaultSettings = _config.Provider.GetSettings(currentEntityType, _config.DefaultEntityName);
            DataTable dt = this.dataGridView1.DataSource as DataTable;
            if (dt != null)
                foreach (DataRow row in dt.Rows)
                    if (defaultSettings.ContainsKey(row["settingName"].ToString()))
                        row["value"] = defaultSettings[row["settingName"].ToString()];



        }

        public string currentEntityType { get { return _currentEntityType; } }

        public string currentEntityName { get { return _currentEntityName; } }

        private void cmbEntityNames_BeforeUpdate(object sender, CancelEventArgs e)
        {
            ConfirmOrAbandonChanges(e);
        }

        private void cmbEntityTypes_BeforeUpdate(object sender, CancelEventArgs e)
        {
            ConfirmOrAbandonChanges(e);
        }

        private void ConfirmOrAbandonChanges(CancelEventArgs e)

        {
            if (_dirty)
            {
                DialogResult result = ApplyPendingChanges();
                if (result == DialogResult.Cancel) { e.Cancel = true; return; }
                if (result == DialogResult.No) AbandonChanges();
                if (result == DialogResult.Yes) SaveChanges();
            }
        }

        private void SaveChanges()
        {

            foreach (DataRow row in (this.dataGridView1.DataSource as DataTable).Rows)
                _config.Provider.SetSetting(currentEntityType, currentEntityName, row["settingName"].ToString(), row["value"].ToString());
            SetDirty(false);

        }

        private void AbandonChanges(bool Rebind = false)
        {
            SetDirty(false);
            if (Rebind)
                SetDataGrid();
        }

        private DialogResult ApplyPendingChanges()

        {
            return MessageBox.Show("There are pending changes to apply - do you want to APPLY them?", "Pending changes", MessageBoxButtons.YesNoCancel);
        }

        private void SetDirty(bool Dirty)
        {
            _dirty = Dirty;
            this.cmdCancel.Enabled = _dirty;
            this.cmdSave.Enabled = Dirty;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            SetDirty(true);
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            AbandonChanges(true);
        }

        private void txtNewEntityName_TextChanged(object sender, EventArgs e)
        {
            this.cmdCreateNewEntity.Enabled = (this.txtNewEntityName.Text != "");
        }

    }
}
