using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conductor.RegexTools

{

    public class ETL_Strategy
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string Query { get; set; }
        List<TranformationComponent> _Components = new List<TranformationComponent>();
        public List<TranformationComponent> Components { get { return _Components; } }
    }




 




    
}
