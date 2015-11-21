using JobSpawn.Master.Controller;
using JobSpawn.Master.Serializers;

namespace JobSpawn.Master
{
    public class TestProxy
    {
        private readonly ISpawnController spawnController;
        private readonly IMessageSerializer serializer;

        public TestProxy(ISpawnController spawnController, IMessageSerializer serializer)
        {
            this.spawnController = spawnController;
            this.serializer = serializer;
        }

        public void DoSomething()
        {
            object[] array = { };
            byte[] messageBytes = serializer.SerialiseMessage(array);
            spawnController.StartRequest(messageBytes);
        }
    }
}
