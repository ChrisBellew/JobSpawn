using System;
using System.Text;
using Newtonsoft.Json;

namespace JobSpawn.Serializers
{
    public class JsonSerializer : IMessageSerializer
    {
        public byte[] SerialiseMessage(object message)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
        }

        public object DeserialiseMessage(byte[] messageBytes, Type messageType)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(messageBytes), messageType);
        }
    }
}
