using System;

namespace JobSpawn.Host
{
    public interface IHostBuilder
    {
        IHost BuildHost(Type concreteType);
    }
}