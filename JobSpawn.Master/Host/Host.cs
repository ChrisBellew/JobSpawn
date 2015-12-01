using System;
using System.Linq;
using JobSpawn.Message;
using JobSpawn.Serializers;
using Newtonsoft.Json;

namespace JobSpawn.Host
{
    public class Host : IHost
    {
        private readonly object instance;
        //private readonly IMessageSerializer serializer;
        private readonly IMessageTypeBuilder messageTypeBuilder;
        private readonly Type instanceType;

        public Host(object instance)
        {
            this.instance = instance;
            //this.serializer = serializer;
            this.messageTypeBuilder = new MessageTypeBuilder();
            instanceType = instance.GetType();
        }

        public object RunMessage(string action, MessageTypeDefinition messageTypeDefinition, object argumentsObj)
        {
            var message = JsonConvert.DeserializeObject(argumentsObj.ToString(), messageTypeBuilder.BuildMessageType(messageTypeDefinition));

            //var message = serializer.DeserialiseMessage(messageBytes, messageTypeBuilder.BuildMessageType(messageTypeDefinition));
            var arguments = messageTypeDefinition.Arguments.Select(x => message.GetType().GetField(x.Name).GetValue(message)).ToArray();
            return instanceType.GetMethod(action).Invoke(instance, arguments);
        }
    }
}
