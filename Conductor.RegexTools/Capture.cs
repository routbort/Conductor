using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conductor.RegexTools
{
    public class Capture
    {
        public string name { get; set; }
        public string value { get; set; }
        public int position { get; set; }
        public int length { get; set; }

        public Capture(string text)
        { this.name = text; }
        private Capture() { }
    }

}
