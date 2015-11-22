using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace JobSpawn.Utility
{
    public class TypeBuilderHelper
    {
        public static AssemblyBuilder CreateAssemblyBuilder(string typeName)
        {
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = typeName + "Assembly";
            return Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
        }

        public static TypeBuilder CreateTypeBuilder(AssemblyBuilder assemblyBuilder, string typeName)
        {
            ModuleBuilder module = assemblyBuilder.DefineDynamicModule(typeName + "Module", typeName + "Module.dll");
            return module.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class);
        }
    }
}
