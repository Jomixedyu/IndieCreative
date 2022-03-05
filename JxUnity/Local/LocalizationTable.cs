using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JxUnity.Local
{

    public class LocalizationTable
    {
        public string InstanceOfClass { get; set; }
        public string Author { get; set; }
        //给人看的 没什么用
        public string Version { get; set; }
        public Dictionary<string, string> Records { get; set; }

    }
}
