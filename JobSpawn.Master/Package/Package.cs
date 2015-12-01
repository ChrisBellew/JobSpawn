using System.Collections.Generic;

namespace JobSpawn.Package
{
    public class Package
    {
        public IEnumerable<PackageAssembly> Assemblies;
        public string EntryAssembly;
        public string EntryType;
    }
}
