using System.Text;
using JobSpawn.Master.Controller;
using JobSpawn.Serializers.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JobSpawn.UnitTests.ProxyBuilder
{
    [TestClass]
    public class ProxyBuilderTests
    {
        [TestMethod]
        public void TestSimpleProxy()
        {
            MockSpawnController spawnController = new MockSpawnController();
            JsonSerializer serializer = new JsonSerializer();
            Master.Proxy.ProxyBuilder proxyBuilder = new Master.Proxy.ProxyBuilder(serializer, spawnController);
            var proxy = proxyBuilder.BuildProxy<IMockType>();
            proxy.DoSomething(1, 2, "a string", new MockObject { AString = "another string", ANumber = 3 });
            Assert.AreEqual("[1,2,\"a string\",{\"AString\":\"another string\",\"ANumber\":3}]", Encoding.UTF8.GetString(spawnController.messageBytes));
        }
        
        public class MockSpawnController : ISpawnController
        {
            public byte[] messageBytes;
            public void StartRequest(byte[] messageBytes)
            {
                this.messageBytes = messageBytes;
            }
        }

        public interface IMockType
        {
            void DoSomething(int numberOne, int numberTwo, string aString, MockObject mockObject);
        }

        public class MockObject
        {
            public string AString;
            public int ANumber;
        }
    }
}
