using System.Collections.Generic;
using JobSpawn.Controller;
using JobSpawn.Proxy;
using JobSpawn.Utility;

namespace JobSpawn
{
    public class Spawner
    {
        public static Spawn CreateSpawn<TConcrete>()
        {
            var instanceContext = DependencyInjector.CreateInstanceContext();
            instanceContext.GetInstance<ISpawnController>(new Dictionary<string, object> { { "concreteType", typeof(TConcrete) } });
            var proxyBuilder = instanceContext.GetInstance<IProxyBuilder>();
            return new Spawn(proxyBuilder);
        }
    }
}
