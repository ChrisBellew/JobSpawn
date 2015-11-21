using System.Text;
using JobSpawn.Master.Serializers;
using Newtonsoft.Json;

namespace JobSpawn.Serializers.Json
{
    public class JsonSerializer : IMessageSerializer
    {
        public byte[] SerialiseMessage(object[] arguments)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(arguments));
        }

        public object[] DeserialiseMessage(byte[] messageBytes)
        {
            return JsonConvert.DeserializeObject<object[]>(Encoding.UTF8.GetString(messageBytes));
        }
    }
}
