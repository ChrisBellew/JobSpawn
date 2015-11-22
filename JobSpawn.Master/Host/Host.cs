using System;
using System.Linq;
using JobSpawn.Message;
using JobSpawn.Serializers;

namespace JobSpawn.Host
{
    public class Host : IHost
    {
        private readonly object instance;
        private readonly IMessageSerializer serializer;
        private readonly IMessageTypeBuilder messageTypeBuilder;
        private readonly Type instanceType;

        public Host(object instance, IMessageSerializer serializer, IMessageTypeBuilder messageTypeBuilder)
        {
            this.instance = instance;
            this.serializer = serializer;
            this.messageTypeBuilder = messageTypeBuilder;
            instanceType = instance.GetType();
        }

        public void RunMessage(string action, MessageTypeDefinition messageTypeDefinition, byte[] messageBytes)
        {
            var message = serializer.DeserialiseMessage(messageBytes, messageTypeBuilder.BuildMessageType(messageTypeDefinition));
            var arguments = messageTypeDefinition.Arguments.Select(x => message.GetType().GetField(x.Name).GetValue(message)).ToArray();
            instanceType.GetMethod(action).Invoke(instance, arguments);
        }
    }
}
