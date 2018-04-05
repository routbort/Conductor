using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conductor.RegexTools
{
    public class TransformationResult
    {
        public List<DataFragment> Results { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
