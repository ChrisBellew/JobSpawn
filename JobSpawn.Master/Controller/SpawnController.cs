using System;
using System.Threading.Tasks;
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

        public Task<object> StartRequest(string action, MessageTypeDefinition messageTypeDefinition, byte[] messageBytes)
        {
            var result = host.RunMessage(action, messageTypeDefinition, messageBytes);
            return Task.FromResult(result);
        }
    }
}
