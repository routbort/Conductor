using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationSettings
{
    public class MachineSetting : SettingsBase
    {
        public override string EntityType
        {
            get
            {
                return "M";
            }
        }
    }
}
