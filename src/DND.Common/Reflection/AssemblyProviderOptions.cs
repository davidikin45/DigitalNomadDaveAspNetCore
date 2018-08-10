using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Reflection
{
    public class AssemblyProviderOptions
    {
        public string BinPath { get; set; }
        public Func<string, Boolean> AssemblyFilter { get; set; }
    }
}
