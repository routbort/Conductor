using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conductor.RegexTools
{


    public class ExactMatchReplaceTransformer : iTransform
    {

        Dictionary<string, string> _Transforms = null;

        public void Initializer(string initializer)
        {
            throw new NotImplementedException();
        }

        public TransformationResult Transform(DataFragment input)
        {
            if (_Transforms == null)
                throw new ApplicationException("MatchReplaceTransformer has not been initializer");
            string output = null;
            foreach (string key in _Transforms.Keys)
                if (input.value == key)
                {
                    output = _Transforms[key];
                    break;
                }
            TransformationResult tr = new TransformationResult();
            tr.Success = (output != null);
            tr.ErrorMessage = (tr.Success) ? "" : "Not found";
            if (tr.Success)
            {
                tr.Results = new List<DataFragment>();
                DataFragment outputFragment = input.ShallowCopy();
                outputFragment.value = output;
                tr.Results.Add(outputFragment);
            }

            return tr;
        }


    }


}
