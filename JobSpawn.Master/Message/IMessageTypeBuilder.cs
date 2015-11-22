using System;

namespace JobSpawn.Message
{
    public interface IMessageTypeBuilder
    {
        Type BuildMessageType(MessageTypeDefinition messageTypeDefinition);
    }
}