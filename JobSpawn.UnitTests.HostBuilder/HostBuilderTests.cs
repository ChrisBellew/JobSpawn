using System;
using System.Text;
using System.Collections.Generic;
using JobSpawn.Message;
using JobSpawn.Serializers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JobSpawn.UnitTests.HostBuilder
{
    [TestClass]
    public class HostBuilderTests
    {
        public static string DatumOne;
        public static int DatumTwo;

        [TestMethod]
        public void TestSimpleHost()
        {
            /*Host.HostBuilder hostBuilder = new Host.HostBuilder(new JsonSerializer(), new MessageTypeBuilder());
            var host = hostBuilder.BuildHost(typeof(MockType));
            host.RunMessage("LogData", new MessageTypeDefinition(), new object[] { "A string", 6 });
            Assert.AreEqual("A string", DatumOne);
            Assert.AreEqual(6, DatumTwo);*/
        }

        public class MockType
        {
            public void LogData(string datumOne, int datumTwo)
            {
                DatumOne = datumOne;
                DatumTwo = datumTwo;
            }
        }
    }
}
