using JobSpawn.Controller;
using JobSpawn.Host;
using JobSpawn.Message;
using JobSpawn.Serializers;

namespace JobSpawn
{
    public class TestMessageProxy : ITestMessageProxy
    {
        private IMessageSerializer messageSerializer = new JsonSerializer();
        private ISpawnController spawnController = new SpawnController(typeof(TestMessageProxy));

        public int DoSomething(int one, string two)
        {
            var testMessage = new TestMessage { one = one, two = two };
            byte[] messageBytes = messageSerializer.SerialiseMessage(testMessage);
            var messageTypeDefinition = new MessageTypeDefinition { Arguments = new [] { new MessageArgument { Name = "one", Type = typeof(int).FullName }, new MessageArgument { Name = "two", Type = typeof(int).FullName } } };
            return (int) spawnController.StartRequest("DoSomething", messageTypeDefinition, messageBytes).Result;
        }
    }

    public interface ITestMessageProxy
    {
        int DoSomething(int one, string two);
    }

    public class TestMessage
    {
        public int one;
        public string two;
    }
}
