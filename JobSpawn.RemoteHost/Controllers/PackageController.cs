using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using JobSpawn.Package;
using JobSpawn.Utility;

namespace JobSpawn.RemoteHost.Controllers
{
    public class PackageController : ApiController
    {
        private string deploymentDirectory = @"c:\temp";

        public void Post(Package.Package package)
        {
            package.Assemblies.ForEach(SaveResource);
            var entryAssembly = LoadAssembly(package.EntryAssembly);
            var entryType = entryAssembly.GetTypes().Single(x => x.FullName.Equals(package.EntryType));
            Startup.Host = new Host.Host(Activator.CreateInstance(entryType));
        }

        private void SaveResource(PackageAssembly packageAssembly)
        {
            File.WriteAllBytes(GetAssemblyPath(packageAssembly.AssemblyName), packageAssembly.AssemblyBytes);
        }

        private Assembly LoadAssembly(string assemblyName)
        {
            return Assembly.LoadFrom(GetAssemblyPath(assemblyName));
        }

        private string GetAssemblyPath(string assemblyName)
        {
            return Path.Combine(deploymentDirectory, assemblyName);
        }
    }
}