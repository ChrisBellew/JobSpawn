using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JobSpawn.Common
{
    public static class ProxyFactory
    {
        public static SpawnProxy CreateProxy()
        {
            return new SpawnProxy();
        }

        private static Dictionary<Type, OpCode> LoadTypeOpCodes = new Dictionary<Type, OpCode>
        {
            { typeof(int), OpCodes.Ldc_I4 }
        };
    }


    public class SpawnProxy
    {
        public TContract As<TContract>()
        {
            var type = typeof(TContract);

            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "JobSpawnProxyAssembly";
            AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder module = assemblyBuilder.DefineDynamicModule("JobSpawnProxyModule", "module.dll");
            TypeBuilder typeBuilder = module.DefineType("JobSpawnProxy", TypeAttributes.Public | TypeAttributes.Class);
            typeBuilder.AddInterfaceImplementation(type);

            foreach (var methodInfo in type.GetMethods())
            {
                var method = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final, methodInfo.ReturnType, methodInfo.GetParameters().Select(x => x.ParameterType).ToArray());

                var ilGenerator = method.GetILGenerator();
                ilGenerator.DeclareLocal(typeof(int));

                ilGenerator.Emit(OpCodes.Nop);
                ilGenerator.Emit(OpCodes.Ldc_I4_3);
                ilGenerator.Emit(OpCodes.Stloc_0);
                Label targetInstruction = ilGenerator.DefineLabel();
                ilGenerator.Emit(OpCodes.Br_S, targetInstruction);
                ilGenerator.MarkLabel(targetInstruction);
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ret);

                typeBuilder.DefineMethodOverride(method, methodInfo);
            }

            var proxyType = typeBuilder.CreateType();

            assemblyBuilder.Save("assembly.dll");

            return (TContract)Activator.CreateInstance(proxyType);
        }
    }
}
