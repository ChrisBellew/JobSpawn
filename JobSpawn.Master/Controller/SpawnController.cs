using System;
using JobSpawn.Host;
using JobSpawn.Message;

namespace JobSpawn.Controller
{
    public class SpawnController : ISpawnController
    {
        private readonly IHost host;

        public SpawnController(IHostBuilder hostBuilder, Type concreteType)
        {
            host = hostBuilder.BuildHost(concreteType);
        }

        public void StartRequest(string action, MessageTypeDefinition messageTypeDefinition, byte[] messageBytes)
        {
            host.RunMessage(action, messageTypeDefinition, messageBytes);
        }
    }
}
