using JobSpawn.Message;

namespace JobSpawn.Controller
{
    public interface ISpawnController
    {
        void StartRequest(string action, MessageTypeDefinition messageTypeDefinition, byte[] messageBytes);
    }
}