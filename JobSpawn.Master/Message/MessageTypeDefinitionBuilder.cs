using System.Linq;
using System.Reflection;

namespace JobSpawn.Message
{
    public class MessageTypeDefinitionBuilder : IMessageTypeDefinitionBuilder
    {
        public MessageTypeDefinition BuildMessageTypeDefinition(MethodInfo methodInfo)
        {
            var obj = new MessageTypeDefinition()
            {
                Arguments = methodInfo.GetParameters().Select(x => new MessageArgument { Name = x.Name, Type = x.ParameterType.AssemblyQualifiedName }).ToArray()
            };
            return obj;
        } 
    }
}