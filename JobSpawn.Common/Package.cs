using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSpawn.Common
{
    public class Package
    {
        public string MainAssembly;
        public string EntryClass;
        public IEnumerable<string> DependencyPaths;
    }
}
