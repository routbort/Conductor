using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace   Conductor.Configuration
{
    public partial class frmTest : Form
    {



        public frmTest()
        {
            InitializeComponent();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            SQLConfigurationProvider provider = new SQLConfigurationProvider();
            provider.Initialize(this.txtConnectionString.Text);
            Config3LevelExample config = new Config3LevelExample(provider);
            this.settingsControl1.Bind(config);
        }
    }
}
