using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
                var parameters = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();
                var method = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final, methodInfo.ReturnType, parameters);

                var ilGenerator = method.GetILGenerator();

                ilGenerator.DeclareLocal(typeof(object[]));
                ilGenerator.DeclareLocal(typeof(string));

                ilGenerator.Emit(OpCodes.Ldc_I4_3);
                ilGenerator.Emit(OpCodes.Newarr, typeof(object));
                ilGenerator.Emit(OpCodes.Dup);
                ilGenerator.Emit(OpCodes.Ldc_I4_0);
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Box, typeof(int));
                ilGenerator.Emit(OpCodes.Stelem_Ref);
                ilGenerator.Emit(OpCodes.Dup);
                ilGenerator.Emit(OpCodes.Ldc_I4_1);
                ilGenerator.Emit(OpCodes.Ldarg_2);
                ilGenerator.Emit(OpCodes.Box, typeof(int));
                ilGenerator.Emit(OpCodes.Stelem_Ref);
                ilGenerator.Emit(OpCodes.Dup);
                ilGenerator.Emit(OpCodes.Ldc_I4_2);
                ilGenerator.Emit(OpCodes.Ldarg_3);
                ilGenerator.Emit(OpCodes.Box, typeof(int));
                ilGenerator.Emit(OpCodes.Stelem_Ref);
                ilGenerator.Emit(OpCodes.Stloc_0);
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Call, typeof(JsonConvert).GetMethod("SerializeObject", new [] { typeof(object) }));
                ilGenerator.Emit(OpCodes.Stloc_1);
                Label targetInstruction = ilGenerator.DefineLabel();
                ilGenerator.Emit(OpCodes.Br_S, targetInstruction);
                ilGenerator.MarkLabel(targetInstruction);
                ilGenerator.Emit(OpCodes.Ldloc_1);
                ilGenerator.Emit(OpCodes.Ret);

                /*ilGenerator.DeclareLocal(typeof(int));
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Stloc_0);
                Label targetInstruction = ilGenerator.DefineLabel();
                ilGenerator.Emit(OpCodes.Br_S, targetInstruction);
                ilGenerator.MarkLabel(targetInstruction);
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ret);*/



                /*ilGenerator.DeclareLocal(typeof(int));
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Ldarg_2);
                ilGenerator.Emit(OpCodes.Add);
                ilGenerator.Emit(OpCodes.Ret);*/

                /*ilGenerator.DeclareLocal(typeof(int));
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Ret);*/

                /*ilGenerator.DeclareLocal(typeof(int));
                ilGenerator.Emit(OpCodes.Ldc_I4_3);
                ilGenerator.Emit(OpCodes.Ret);*/

                typeBuilder.DefineMethodOverride(method, methodInfo);
            }

            var proxyType = typeBuilder.CreateType();

            assemblyBuilder.Save("assembly.dll");

            return (TContract)Activator.CreateInstance(proxyType);
        }
    }
}
