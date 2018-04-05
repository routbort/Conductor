using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conductor.RegexTools
{
    public class DataFragment
    {
        //public string patient_identifier { get; set; }
        //public string specimen_identifier { get; set; }
        //public string source_identifier { get; set; }
        public string attribute { get; set; }
        public string subattribute { get; set; }
        public string value { get; set; }
        public string subvalue { get; set; }

        public DataFragment ShallowCopy()
        {
            return (DataFragment)this.MemberwiseClone();
        }

    }

}
