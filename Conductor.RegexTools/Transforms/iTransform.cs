using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conductor.RegexTools
{
    public interface iTransform
    {
        void Initializer(string initializer);
        TransformationResult Transform(DataFragment input);
    }
}
