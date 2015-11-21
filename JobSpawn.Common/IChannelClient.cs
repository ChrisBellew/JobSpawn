using System.Threading.Tasks;

namespace JobSpawn.Common
{
    public interface IChannelClient
    {
        void SendRequest(string jsonArgs);
    }
}