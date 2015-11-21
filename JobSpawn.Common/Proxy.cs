using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JobSpawn.Common
{
    public static class ProxyFactory
    {
        public static SpawnProxy CreateProxy()
        {
            return new SpawnProxy();
        }

        private static Dictionary<Type, OpCode> LoadTypeOpCodes = new Dictionary<Type, OpCode>
        {
            { typeof(int), OpCodes.Ldc_I4 }
        };
    }
    
    public class SpawnProxy
    {
        public TContract As<TContract>()
        {
            return default(TContract);

            /*var contractType = typeof(TContract);

            ConcreteProxyBuilder concreteProxyBuilder = new ConcreteProxyBuilder();
            var concreteProxy = concreteProxyBuilder.CreateConcreteProxy(contractType);

            return (TContract)Activator.CreateInstance(concreteProxy);*/
        }
    }
}
