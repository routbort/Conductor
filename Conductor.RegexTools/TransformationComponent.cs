using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conductor.RegexTools
{

    public class TranformationComponent
    {
        public string Pattern { get; set; }
        public bool IsRequiredToMatch { get; set; }
        public bool AllowMultipleMatches { get; set; }


    }
}
