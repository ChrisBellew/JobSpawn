using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JobSpawn.Common
{
    public static class Spawner
    {
        public static SpawnProxy Spawn<T>()
        {
            /*var entryClass = typeof(T);
            var location = entryClass.Assembly.Location;
            var dependencies = entryClass.GetReferencesAssembliesPaths();

            Package package = new Package
            {
                MainAssembly = location,
                EntryClass = entryClass.FullName,
                DependencyPaths = dependencies
            };

            var spawnController = new SpawnController("http://localhost:9000/");
            spawnController.SendPackage(package);*/
            
            return ProxyFactory.CreateProxy();
        }

        public static IEnumerable<T> SpawnMany<T>(int number) where T : new()
        {
            return Enumerable.Range(0, number).Select(x => new T());
        }

        public static IEnumerable<string> GetReferencesAssembliesPaths(this Type type)
        {
            yield return type.Assembly.Location;

            foreach (AssemblyName assemblyName in type.Assembly.GetReferencedAssemblies())
            {
                yield return Assembly.ReflectionOnlyLoad(assemblyName.FullName).Location;
            }
        }
    }
}
