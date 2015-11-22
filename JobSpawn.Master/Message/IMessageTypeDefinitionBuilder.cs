using System.Reflection;

namespace JobSpawn.Message
{
    public interface IMessageTypeDefinitionBuilder
    {
        MessageTypeDefinition BuildMessageTypeDefinition(MethodInfo methodInfo);
    }
}