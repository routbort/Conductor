using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Conductor.RegexTools
{
  public static  class RegexHelper
    {

       public static List<string> GetNamedCaptureGroups(string pattern)
        {
            List<string> results = new List<string>();
            SafeRegex re = new SafeRegex(@"\(\?\<(?<capture>.*?)\>.*?\)");
            MatchCollection mc = re.Matches(pattern);
            foreach (Match m in mc)
                results.Add(m.Groups["capture"].Value);
            return results;
        }


    }
}
