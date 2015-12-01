using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using JobSpawn.Controller;
using JobSpawn.Message;
using JobSpawn.Serializers;
using JobSpawn.Utility;

namespace JobSpawn.Proxy
{
    public class ProxyBuilder : IProxyBuilder
    {
        private readonly IMessageSerializer serializer;
        private readonly ISpawnController spawnController;
        private readonly IMessageTypeBuilder messageTypeBuilder;
        private readonly IMessageTypeDefinitionBuilder messageTypeDefinitionBuilder;

        public ProxyBuilder(IMessageSerializer serializer, ISpawnController spawnController, IMessageTypeBuilder messageTypeBuilder, IMessageTypeDefinitionBuilder messageTypeDefinitionBuilder)
        {
            this.serializer = serializer;
            this.spawnController = spawnController;
            this.messageTypeBuilder = messageTypeBuilder;
            this.messageTypeDefinitionBuilder = messageTypeDefinitionBuilder;
        }

        public TContract BuildProxy<TContract>()
        {
            var contractType = typeof(TContract);

            // Build the concrete implementation of the contract to act as the proxy for messages
            AssemblyBuilder assemblyBuilder = TypeBuilderHelper.CreateAssemblyBuilder("JobSpawnProxy");
            TypeBuilder typeBuilder = TypeBuilderHelper.CreateTypeBuilder(assemblyBuilder, "JobSpawnProxy");

            FieldBuilder spawnControllerField = typeBuilder.DefineField("spawnController", typeof(ISpawnController), FieldAttributes.Private);
            FieldBuilder serializerField = typeBuilder.DefineField("serializer", typeof(IMessageSerializer), FieldAttributes.Private);

            BuildConstructor(typeBuilder, spawnControllerField, serializerField);
            typeBuilder.AddInterfaceImplementation(contractType);
            contractType.GetMethods().ForEach(methodInfo => BuildConcreteMethod(typeBuilder, methodInfo, spawnControllerField, serializerField));

            // Create an instance of the proxy
            var proxyType = typeBuilder.CreateType();
            assemblyBuilder.Save("JobSpawnProxyAssembly.dll");
            return (TContract)Activator.CreateInstance(proxyType, spawnController, serializer);
        }

        private void BuildConstructor(TypeBuilder typeBuilder, FieldBuilder spawnControllerField, FieldBuilder serializerField)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new [] { typeof(ISpawnController), typeof(IMessageSerializer) });

            var ilGenerator = constructorBuilder.GetILGenerator();

            // Load 'this' onto the stack
            ilGenerator.Emit(OpCodes.Ldarg_0);

            // Call the constructor on System.object
            var baseCtor = typeof(object).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);
            ilGenerator.Emit(OpCodes.Call, baseCtor);
            
            // Load 'this' onto the stack
            ilGenerator.Emit(OpCodes.Ldarg_0);

            // Load the spawn controller onto the stack
            ilGenerator.Emit(OpCodes.Ldarg_1);

            // Save the spawn controller into the instance field
            ilGenerator.Emit(OpCodes.Stfld, spawnControllerField);

            // Load 'this' onto the stack
            ilGenerator.Emit(OpCodes.Ldarg_0);

            // Load the serializer onto the stack
            ilGenerator.Emit(OpCodes.Ldarg_2);

            // Save the serializer into the instance field
            ilGenerator.Emit(OpCodes.Stfld, serializerField);

            // Return nothing
            ilGenerator.Emit(OpCodes.Ret);
        }

        private void BuildConcreteMethod(TypeBuilder typeBuilder, MethodInfo methodInfo, FieldBuilder spawnControllerField, FieldBuilder serializerField)
        {
            var parameters = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();
            var method = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final, methodInfo.ReturnType, parameters);
            typeBuilder.DefineMethodOverride(method, methodInfo);

            var messageTypeDefinition = messageTypeDefinitionBuilder.BuildMessageTypeDefinition(methodInfo);
            var messageType = messageTypeBuilder.BuildMessageType(messageTypeDefinition);

            var ilGenerator = method.GetILGenerator();

            ilGenerator.DeclareLocal(messageType);
            ilGenerator.DeclareLocal(typeof(byte[]));
            ilGenerator.DeclareLocal(typeof(MessageTypeDefinition));
            ilGenerator.DeclareLocal(messageType);
            ilGenerator.DeclareLocal(typeof(MessageTypeDefinition));
            ilGenerator.DeclareLocal(typeof(MessageArgument));
            if (methodInfo.ReturnType != typeof (void))
            {
                ilGenerator.DeclareLocal(methodInfo.ReturnType);
            }

            ilGenerator.Emit(OpCodes.Newobj, messageType.GetConstructors()[0]);
            ilGenerator.Emit(OpCodes.Stloc_3);

            var index = 0;
            foreach (var argument in messageTypeDefinition.Arguments)
            {
                ilGenerator.Emit(OpCodes.Ldloc_3);
                ilGenerator.Emit(OpCodes.Ldarg, index + 1);
                ilGenerator.Emit(OpCodes.Stfld, messageType.GetField(argument.Name));
                index++;
            }
            
            /*ilGenerator.Emit(OpCodes.Ldloc_3);
            ilGenerator.Emit(OpCodes.Stloc_0);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, serializerField);
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Callvirt, serializer.GetType().GetMethod("SerialiseMessage"));*/
            //ilGenerator.Emit(OpCodes.Stloc_1);
            ilGenerator.Emit(OpCodes.Newobj, typeof(MessageTypeDefinition).GetConstructors()[0]);
            ilGenerator.Emit(OpCodes.Stloc_S, 4);
            ilGenerator.Emit(OpCodes.Ldloc_S, 4);
            ilGenerator.Emit(OpCodes.Ldc_I4, messageTypeDefinition.Arguments.Length);
            ilGenerator.Emit(OpCodes.Newarr, typeof(MessageArgument));


            index = 0;
            foreach (var argument in messageTypeDefinition.Arguments)
            {
                ilGenerator.Emit(OpCodes.Dup);
                ilGenerator.Emit(OpCodes.Ldc_I4, index);
                ilGenerator.Emit(OpCodes.Newobj, typeof(MessageArgument).GetConstructors()[0]);
                ilGenerator.Emit(OpCodes.Stloc_S, 5);
                ilGenerator.Emit(OpCodes.Ldloc_S, 5);
                ilGenerator.Emit(OpCodes.Ldstr, argument.Name);
                ilGenerator.Emit(OpCodes.Stfld, typeof(MessageArgument).GetField("Name"));
                ilGenerator.Emit(OpCodes.Ldloc_S, 5);
                ilGenerator.Emit(OpCodes.Ldtoken, Type.GetType(argument.Type));
                ilGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", new[] { typeof(RuntimeTypeHandle) }));
                ilGenerator.Emit(OpCodes.Callvirt, typeof(Type).GetMethod("get_FullName"));
                ilGenerator.Emit(OpCodes.Stfld, typeof(MessageArgument).GetField("Type"));
                ilGenerator.Emit(OpCodes.Ldloc_S, 5);
                ilGenerator.Emit(OpCodes.Stelem_Ref);
                index++;
            }

            ilGenerator.Emit(OpCodes.Stfld, typeof(MessageTypeDefinition).GetField("Arguments"));
            ilGenerator.Emit(OpCodes.Ldloc_S, 4);
            ilGenerator.Emit(OpCodes.Stloc_2);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, spawnControllerField);
            ilGenerator.Emit(OpCodes.Ldstr, methodInfo.Name);
            ilGenerator.Emit(OpCodes.Ldloc_2);
            //ilGenerator.Emit(OpCodes.Ldloc_1);
            ilGenerator.Emit(OpCodes.Ldloc_3);
            ilGenerator.Emit(OpCodes.Callvirt, spawnController.GetType().GetMethod("StartRequest"));
            if (methodInfo.ReturnType != typeof (void))
            {
                ilGenerator.Emit(OpCodes.Callvirt, typeof (Task<object>).GetMethod("get_Result"));
                if (methodInfo.ReturnType.IsValueType)
                {
                    ilGenerator.Emit(OpCodes.Unbox_Any, methodInfo.ReturnType);
                }
                ilGenerator.Emit(OpCodes.Stloc_S, 6);
                ilGenerator.Emit(OpCodes.Ldloc_S, 6);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Pop);
            }
            ilGenerator.Emit(OpCodes.Ret);
        }
    }
}
