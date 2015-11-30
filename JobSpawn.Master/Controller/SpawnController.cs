using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using JobSpawn.Client;
using JobSpawn.Host;
using JobSpawn.Message;

namespace JobSpawn.Controller
{
    public class SpawnController : ISpawnController
    {
        private readonly Type concreteType;
        private readonly IHost host;

        public SpawnController(IHostBuilder hostBuilder, Type concreteType)
        {
            this.concreteType = concreteType;
            host = hostBuilder.BuildHost(concreteType);
        }

        public async Task<object> StartRequest(string action, MessageTypeDefinition messageTypeDefinition, byte[] messageBytes)
        {
            var message = new Message.Message { Action = action, MessageTypeDefinition = messageTypeDefinition, MessageBytes = messageBytes };

            MessageClient messageClient = new MessageClient();
            var messageResult = await messageClient.SendMessage(message);

            var returnType = concreteType.GetMethod(action).ReturnType;
            //var obj = ByteArrayToObject(messageResult.Result);

            return Convert.ChangeType(messageResult.Result, returnType);

            //var result = host.RunMessage(action, messageTypeDefinition, messageBytes);
            //var result =  messageResult.Result;
        }

        public static object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();

            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);

            object obj = (object)binForm.Deserialize(memStream);

            return obj;
        }
    }
}
