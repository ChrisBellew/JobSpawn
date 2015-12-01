using System;
using System.Linq;
using System.Threading.Tasks;
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
            //DependencyInjector.AddType<IHostBuilder, HostBuilder>();
            DependencyInjector.AddType<IProxyBuilder, ProxyBuilder>();
            DependencyInjector.AddType<IMessageSerializer, JsonSerializer>();
            DependencyInjector.AddType<IMessageTypeDefinitionBuilder, MessageTypeDefinitionBuilder>();
            DependencyInjector.AddType<IMessageTypeBuilder, MessageTypeBuilder>();
            
            /*var mockType = Spawner.CreateSpawn<MockType>().As<IMockType>();
            mockType.LogData("one", 2);
            Assert.AreEqual("one", DatumOne);
            Assert.AreEqual(2, DatumTwo);*/

            var mockType2 = Spawner.CreateSpawn<MockType2>().As<IMockType2>();

            /*Parallel.ForEach(Enumerable.Range(0, 100), new ParallelOptions {MaxDegreeOfParallelism = 10}, state =>
            {*/
                var result = mockType2.LogData("one", 2);
                Assert.AreEqual(42, result);
            //});

            var result2 = mockType2.LogData("one", 2);
            //Assert.AreEqual("one", DatumOne);
            //Assert.AreEqual(2, DatumTwo);
            Assert.AreEqual(42, result2);
        }

        public class MockType : IMockType
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

        public class MockType2 : IMockType2
        {
            public int LogData(string datumOne, int datumTwo)
            {
                DatumOne = datumOne;
                DatumTwo = datumTwo;
                return 42;
            }
        }

        public interface IMockType2
        {
            int LogData(string datumOne, int datumTwo);
        }
    }
}
