using JobSpawn.Message;

namespace JobSpawn.Host
{
    public interface IHost
    {
        object RunMessage(string action, MessageTypeDefinition messageTypeDefinition, object argumentsObj);
    }
}