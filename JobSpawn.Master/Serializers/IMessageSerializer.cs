namespace JobSpawn.Master.Serializers
{
    public interface IMessageSerializer
    {
        byte[] SerialiseMessage(object[] arguments);
    }
}