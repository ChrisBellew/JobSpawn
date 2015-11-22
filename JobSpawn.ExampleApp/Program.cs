using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobSpawn.Controller;
using JobSpawn.Host;
using JobSpawn.Message;
using JobSpawn.Proxy;
using JobSpawn.Serializers;
using JobSpawn.Utility;

namespace JobSpawn.ExampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DependencyInjector.AddType<IProxyBuilder, ProxyBuilder>();
            DependencyInjector.AddType<ISpawnController, SpawnController>();
            DependencyInjector.AddType<IHostBuilder, HostBuilder>();
            DependencyInjector.AddType<IProxyBuilder, ProxyBuilder>();
            DependencyInjector.AddType<IMessageSerializer, JsonSerializer>();
            DependencyInjector.AddType<IMessageTypeDefinitionBuilder, MessageTypeDefinitionBuilder>();
            DependencyInjector.AddType<IMessageTypeBuilder, MessageTypeBuilder>();

            var proxy = Spawner.CreateSpawn<MyConcreteClass>().As<MyContract>();
            proxy.DoSomething(1, "two");

            Console.ReadKey();
        }
    }

    public interface MyContract
    {
        void DoSomething(int one, string two);
    }

    public class MyConcreteClass : MyContract
    {
        public void DoSomething(int one, string two)
        {
            Console.WriteLine(one);
            Console.WriteLine(two);
            Console.WriteLine("It worked!");
        }
    }
}
