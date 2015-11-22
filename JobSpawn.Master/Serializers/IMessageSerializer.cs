using System;

namespace JobSpawn.Serializers
{
    public interface IMessageSerializer
    {
        byte[] SerialiseMessage(object message);
        object DeserialiseMessage(byte[] arguments, Type messageType);
    }
}