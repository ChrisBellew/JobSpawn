using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using JobSpawn.Master.Controller;
using JobSpawn.Master.Serializers;
using JobSpawn.Master.Utility;

namespace JobSpawn.Master.Proxy
{
    public class ProxyBuilder
    {
        private readonly IMessageSerializer serializer;
        private readonly ISpawnController spawnController;

        public ProxyBuilder(IMessageSerializer serializer, ISpawnController spawnController)
        {
            this.serializer = serializer;
            this.spawnController = spawnController;
        }

        public TContract BuildProxy<TContract>()
        {
            var contractType = typeof(TContract);

            // Build the concrete implementation of the contract to act as the proxy for messages
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "JobSpawnProxyAssembly";
            AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder module = assemblyBuilder.DefineDynamicModule("JobSpawnProxyModule", "testmodule.dll");
            TypeBuilder typeBuilder = module.DefineType("JobSpawnProxy", TypeAttributes.Public | TypeAttributes.Class);
            //TypeBuilder typeBuilder = CreateTypeBuilder();

            FieldBuilder spawnControllerField = typeBuilder.DefineField("spawnController", typeof(ISpawnController), FieldAttributes.Private);
            FieldBuilder serializerField = typeBuilder.DefineField("serializer", typeof(IMessageSerializer), FieldAttributes.Private);

            BuildConstructor(typeBuilder, spawnControllerField, serializerField);
            typeBuilder.AddInterfaceImplementation(contractType);
            contractType.GetMethods().ToList().ForEach(methodInfo => BuildConcreteMethod(typeBuilder, methodInfo, spawnControllerField, serializerField));

            // Create an instance of the proxy
            var proxyType = typeBuilder.CreateType();

            assemblyBuilder.Save("assembly.dll");


            return (TContract)Activator.CreateInstance(proxyType, spawnController, serializer);
        }

        /*private TypeBuilder CreateTypeBuilder()
        {
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "JobSpawnProxyAssembly";
            AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder module = assemblyBuilder.DefineDynamicModule("JobSpawnProxyModule", "testmodule.dll");
            return module.DefineType("JobSpawnProxy", TypeAttributes.Public | TypeAttributes.Class);
        }*/

        private void BuildConstructor(TypeBuilder typeBuilder, FieldBuilder spawnControllerField, FieldBuilder serializerField)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new [] { typeof(ISpawnController), typeof(IMessageSerializer) });

            var ilGenerator = constructorBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);

            var baseCtor = typeof(object).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);
            ilGenerator.Emit(OpCodes.Call, baseCtor);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Stfld, spawnControllerField);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_2);
            ilGenerator.Emit(OpCodes.Stfld, serializerField);
            ilGenerator.Emit(OpCodes.Ret);
        }

        private void BuildConcreteMethod(TypeBuilder typeBuilder, MethodInfo methodInfo, FieldBuilder spawnControllerField, FieldBuilder serializerField)
        {
            var parameters = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();
            var method = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final, methodInfo.ReturnType, parameters);
            typeBuilder.DefineMethodOverride(method, methodInfo);

            var ilGenerator = method.GetILGenerator();

            // Create a local variable to store the object array which will catch all the incoming arguments
            ilGenerator.DeclareLocal(typeof(object[]));
            ilGenerator.DeclareLocal(typeof(byte[]));

            // Build the object array
            ilGenerator.Emit(OpCodes.Ldc_I4, parameters.Length);
            ilGenerator.Emit(OpCodes.Newarr, typeof(object));
            
            // Add the incoming arguments into the object array
            parameters.ForEach((parameter, index) => LoadArgumentIntoObjectArray(ilGenerator, parameter, index));

            // Pass the object array onto the spawn controller
            //ilGenerator.Emit(OpCodes.Call, typeof(ProxyBuilder).GetMethod("MessageCreated"));
            
            // Store the object array as local variable 1
            ilGenerator.Emit(OpCodes.Stloc_0);

            // Load 'this' onto the stack
            ilGenerator.Emit(OpCodes.Ldarg_0);

            // Get the serializer for this instance and puts it on the stack
            ilGenerator.Emit(OpCodes.Ldfld, serializerField);

            // Put the object array on the stack
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Callvirt, serializer.GetType().GetMethod("SerialiseMessage"));
            ilGenerator.Emit(OpCodes.Stloc_1);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, spawnControllerField);
            ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.Emit(OpCodes.Callvirt, spawnController.GetType().GetMethod("StartRequest"));
            ilGenerator.Emit(OpCodes.Ret);
        }

        private void LoadArgumentIntoObjectArray(ILGenerator ilGenerator, Type parameter, int index)
        {
            // Create a duplicate of the object array on the stack, the Stelem_Ref opcode will need this below
            ilGenerator.Emit(OpCodes.Dup);

            // Set up the index for the argument to be placed into the object array
            ilGenerator.Emit(OpCodes.Ldc_I4, index);

            // Load the argument onto the stack
            ilGenerator.Emit(OpCodes.Ldarg, index + 1);
            if (parameter.IsValueType)
            {
                // Value types must be boxed before they can go into the object array
                ilGenerator.Emit(OpCodes.Box, parameter.UnderlyingSystemType);
            }

            // Place the argument into the object array at the index given earlier
            ilGenerator.Emit(OpCodes.Stelem_Ref);
        }

        /*public void MessageCreated(object[] message)
        {
            var messageBytes = serializer.SerialiseMessage(message);
            spawnController.StartRequest(messageBytes);
        }*/
    }
}
