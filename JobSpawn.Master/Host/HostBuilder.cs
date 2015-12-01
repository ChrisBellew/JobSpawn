/*
using System;
using JobSpawn.Message;
using JobSpawn.Serializers;

namespace JobSpawn.Host
{
    public class HostBuilder : IHostBuilder
    {
        private readonly IMessageSerializer serializer;
        private readonly IMessageTypeBuilder messageTypeBuilder;

        public HostBuilder(IMessageSerializer serializer, IMessageTypeBuilder messageTypeBuilder)
        {
            this.serializer = serializer;
            this.messageTypeBuilder = messageTypeBuilder;
        }

        public IHost BuildHost(Type concreteType)
        {
            var instance = Activator.CreateInstance(concreteType);
            return new Host(instance, serializer, messageTypeBuilder);
        }
    }
}
*/
