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
    public class ConcreteProxyBuilder
    {
        private readonly IChannelClientBuilder channelClientBuilder;

        public ConcreteProxyBuilder(IChannelClientBuilder channelClientBuilder)
        {
            this.channelClientBuilder = channelClientBuilder;
        }

        public Type CreateConcreteProxy(Type contractType)
        {
            var channelClient = channelClientBuilder.BuildChannelClient();
            var sendRequestCallback = channelClient.GetType().GetMethod("SendRequest");
            return CreateConcreteType(contractType, sendRequestCallback);
        }

        private Type CreateConcreteType(Type contractType, MethodInfo sendRequestCallback)
        {
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "JobSpawnProxyAssembly";
            AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder module = assemblyBuilder.DefineDynamicModule("JobSpawnProxyModule", "module.dll");
            TypeBuilder typeBuilder = module.DefineType("JobSpawnProxy", TypeAttributes.Public | TypeAttributes.Class);
            typeBuilder.AddInterfaceImplementation(contractType);

            foreach (var methodInfo in contractType.GetMethods())
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
                ilGenerator.Emit(OpCodes.Call, typeof(JsonConvert).GetMethod("SerializeObject", new[] { typeof(object) }));
                ilGenerator.Emit(OpCodes.Stloc_1);
                Label targetInstruction = ilGenerator.DefineLabel();
                ilGenerator.Emit(OpCodes.Br_S, targetInstruction);
                ilGenerator.MarkLabel(targetInstruction);
                ilGenerator.Emit(OpCodes.Ldloc_1);
                ilGenerator.Emit(OpCodes.Ret);

                /*ilGenerator.DeclareLocal(typeof(object[]));
                ilGenerator.DeclareLocal(typeof(string));

                ilGenerator.Emit(OpCodes.Ldc_I4, parameters.Length);
                ilGenerator.Emit(OpCodes.Newarr, typeof(object));

                var index = 0;
                foreach (var parameter in parameters)
                {
                    ilGenerator.Emit(OpCodes.Dup);
                    ilGenerator.Emit(OpCodes.Ldc_I4, index);
                    ilGenerator.Emit(OpCodes.Ldarg, index + 1);
                    if (parameter.IsValueType)
                    {
                        ilGenerator.Emit(OpCodes.Box, parameter.UnderlyingSystemType);
                    }
                    ilGenerator.Emit(OpCodes.Stelem_Ref);
                    index++;
                }

                ilGenerator.Emit(OpCodes.Call, typeof(JsonConvert).GetMethod("SerializeObject", new[] { typeof(object) }));
                //ilGenerator.Emit(OpCodes.Call, sendRequestCallback);
                ilGenerator.Emit(OpCodes.Ret);*/

                typeBuilder.DefineMethodOverride(method, methodInfo);
            }

            var type = typeBuilder.CreateType();
            assemblyBuilder.Save("assembly.dll");
            return type;
        }
    }
}
