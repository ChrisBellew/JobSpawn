using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using JobSpawn.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JobSpawn.ProxyTests
{
    [TestClass]
    public class ConcreteProxyBuilderTests
    {
        [TestMethod]
        public void TestRequestMade()
        {
            ConcreteProxyBuilder concreteProxyBuilder = new ConcreteProxyBuilder(new MockChannelClientBuilder());
            var concreteProxyType = concreteProxyBuilder.CreateConcreteProxy(typeof(IMockAppClass));
            IMockAppClass concreteProxy = (IMockAppClass) Activator.CreateInstance(concreteProxyType);
            concreteProxy.SendNumbers(1, 2, 3);
            Assert.AreEqual("[1,2,3]", MockChannelClientBuilder.SentJsonArgs);
        }

        public class MockChannelClientBuilder : IChannelClientBuilder
        {
            public static string SentJsonArgs;

            public IChannelClient BuildChannelClient()
            {
                return new MockChannelClient();
            }

            public class MockChannelClient : IChannelClient
            {
                public void SendRequest(string jsonArgs)
                {
                    SentJsonArgs = jsonArgs;
                }
            }
        }

        public interface IMockAppClass
        {
            void SendNumbers(int numberOne, int numberTwo, int numberThree);
        }
    }
}
