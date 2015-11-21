namespace JobSpawn.Master.Controller
{
    public interface ISpawnController
    {
        void StartRequest(byte[] messageBytes);
    }
}