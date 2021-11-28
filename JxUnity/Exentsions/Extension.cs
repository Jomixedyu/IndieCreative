using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JxUnity.Exentsions
{
    public class Extension
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public string Version { get; protected set; }
        public string Author { get; private set; }
    }
}
