using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace JobSpawn.Utility
{
    public class TypeBuilderHelper
    {
        // Append IDs to the assembly and module names so they do not overwrite previously generated assemblies and modules
        private static int nextAssemblyId = 1;
        private static int nextTypeId = 1;

        public static AssemblyBuilder CreateAssemblyBuilder(string typeName)
        {
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = typeName + "Assembly" + (nextAssemblyId++);
            return Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
        }

        public static TypeBuilder CreateTypeBuilder(AssemblyBuilder assemblyBuilder, string typeName)
        {
            ModuleBuilder module = assemblyBuilder.DefineDynamicModule(typeName + "Module" + (nextTypeId++), typeName + "Module.dll");
            return module.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class);
        }
    }
}
