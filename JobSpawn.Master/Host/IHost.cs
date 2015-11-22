using JobSpawn.Message;

namespace JobSpawn.Host
{
    public interface IHost
    {
        void RunMessage(string action, MessageTypeDefinition messageTypeDefinition, byte[] messageBytes);
    }
}