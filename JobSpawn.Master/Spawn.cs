using JobSpawn.Proxy;

namespace JobSpawn
{
    public class Spawn
    {
        private readonly IProxyBuilder proxyBuilder;

        public Spawn(IProxyBuilder proxyBuilder)
        {
            this.proxyBuilder = proxyBuilder;
        }

        public TContract As<TContract>()
        {
            return proxyBuilder.BuildProxy<TContract>();
        }
    }
}
