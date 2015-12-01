using System.Text;
using System.Threading.Tasks;
using JobSpawn.Controller;
using JobSpawn.Message;
using JobSpawn.Serializers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JobSpawn.UnitTests.ProxyBuilder
{
    [TestClass]
    public class ProxyBuilderTests
    {

        [TestMethod]
        public void TestReturnValue()
        {
            MockSpawnController spawnController = new MockSpawnController();
            Proxy.ProxyBuilder proxyBuilder = new Proxy.ProxyBuilder(new JsonSerializer(), spawnController, new MessageTypeBuilder(), new MessageTypeDefinitionBuilder());
            var proxy = proxyBuilder.BuildProxy<IMockType>();
            var result = proxy.DoSomethingInt(1);

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void TestNoReturnValue()
        {
            MockSpawnController spawnController = new MockSpawnController();
            Proxy.ProxyBuilder proxyBuilder = new Proxy.ProxyBuilder(new JsonSerializer(), spawnController, new MessageTypeBuilder(), new MessageTypeDefinitionBuilder());
            var proxy = proxyBuilder.BuildProxy<IMockType>();
            proxy.DoSomethingVoid(1);
        }

        [TestMethod]
        public void TestSimpleProxy()
        {
            MockSpawnController spawnController = new MockSpawnController();
            Proxy.ProxyBuilder proxyBuilder = new Proxy.ProxyBuilder(new JsonSerializer(), spawnController, new MessageTypeBuilder(), new MessageTypeDefinitionBuilder());
            var proxy = proxyBuilder.BuildProxy<IMockType>();
            proxy.DoSomethingPrimitiveAndObjectParameters(1, 2, "a string", new MockObject { AString = "another string", ANumber = 3 });
            
            Assert.AreEqual(4, spawnController.messageTypeDefinition.Arguments.Length);
            Assert.AreEqual(typeof(int).FullName, spawnController.messageTypeDefinition.Arguments[0].Type);
            Assert.AreEqual(typeof(int).FullName, spawnController.messageTypeDefinition.Arguments[1].Type);
            Assert.AreEqual(typeof(string).FullName, spawnController.messageTypeDefinition.Arguments[2].Type);
            Assert.AreEqual(typeof(MockObject).FullName, spawnController.messageTypeDefinition.Arguments[3].Type);
            //Assert.AreEqual("{\"one\":1,\"two\":2,\"three\":\"a string\",\"mockObject\":{\"AString\":\"another string\",\"ANumber\":3}}", Encoding.UTF8.GetString(spawnController.arguments));
        }

        public class MockSpawnController : ISpawnController
        {
            public string action;
            public MessageTypeDefinition messageTypeDefinition;
            public object arguments;
            public Task<object> StartRequest(string action, MessageTypeDefinition messageTypeDefinition, object arguments)
            {
                this.action = action;
                this.messageTypeDefinition = messageTypeDefinition;
                this.arguments = arguments;
                return Task.FromResult((object)3);
            }
        }

        public interface IMockType
        {
            int DoSomethingInt(int one);
            void DoSomethingVoid(int one);
            void DoSomethingPrimitiveAndObjectParameters(int one, int two, string three, MockObject mockObject);
        }

        public class MockObject
        {
            public string AString;
            public int ANumber;
        }
    }
}
