using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Conductor.Devices.CABPrinter
{
    public class SimpleSubstitutionHolder
    {

        public string VAR001 { get; set; } 
        public string VAR002 { get; set; }
        public string VAR003 { get; set; }
        public string VAR004 { get; set; }
        public string VAR005 { get; set; }
        public string VAR006 { get; set; }
        public string VAR007 { get; set; }


        public Dictionary<string, string> GetReplacements()
        {

                Dictionary<string, string> output = new Dictionary<string, string>();
                BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
                foreach (PropertyInfo property in this.GetType().GetProperties(flags))
                    output[property.Name] = (property.GetValue(this, null) == null) ? "" : property.GetValue(this, null).ToString();
                return output;

        }
    }
}
