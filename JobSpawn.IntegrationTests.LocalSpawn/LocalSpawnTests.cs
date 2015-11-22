using System;
using JobSpawn.Controller;
using JobSpawn.Host;
using JobSpawn.Message;
using JobSpawn.Proxy;
using JobSpawn.Serializers;
using JobSpawn.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JobSpawn.IntegrationTests.LocalSpawn
{
    [TestClass]
    public class LocalSpawnTests
    {
        public static string DatumOne;
        public static int DatumTwo;

        [TestMethod]
        public void TestSimpleLocalSpawn()
        {
            DependencyInjector.AddType<IProxyBuilder, ProxyBuilder>();
            DependencyInjector.AddType<ISpawnController, SpawnController>();
            DependencyInjector.AddType<IHostBuilder, HostBuilder>();
            DependencyInjector.AddType<IProxyBuilder, ProxyBuilder>();
            DependencyInjector.AddType<IMessageSerializer, JsonSerializer>();
            DependencyInjector.AddType<IMessageTypeDefinitionBuilder, MessageTypeDefinitionBuilder>();
            DependencyInjector.AddType<IMessageTypeBuilder, MessageTypeBuilder>();
            
            var mockType = Spawner.CreateSpawn<MockType>().As<IMockType>();
            mockType.LogData("one", 2);
            Assert.AreEqual("one", DatumOne);
            Assert.AreEqual(2, DatumTwo);
        }

        public class MockType
        {
            public void LogData(string datumOne, int datumTwo)
            {
                DatumOne = datumOne;
                DatumTwo = datumTwo;
            }
        }

        public interface IMockType
        {
            void LogData(string datumOne, int datumTwo);
        }
    }
}
