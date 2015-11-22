using System;
using System.Reflection;
using System.Reflection.Emit;
using JobSpawn.Utility;

namespace JobSpawn.Message
{
    public class MessageTypeBuilder : IMessageTypeBuilder
    {
        public Type BuildMessageType(MessageTypeDefinition messageTypeDefinition)
        {
            AssemblyBuilder assemblyBuilder = TypeBuilderHelper.CreateAssemblyBuilder("JobSpawnMessage");
            TypeBuilder typeBuilder = TypeBuilderHelper.CreateTypeBuilder(assemblyBuilder, "JobSpawnMessage");
            messageTypeDefinition.Arguments.ForEach(argument => typeBuilder.DefineField(argument.Name, Type.GetType(argument.Type), FieldAttributes.Public));
            var type = typeBuilder.CreateType();
            assemblyBuilder.Save("JobSpawnMessageAssembly.dll");
            return type;
        }
    }
}
