using System.Threading.Tasks;
using JobSpawn.Message;

namespace JobSpawn.Controller
{
    public interface ISpawnController
    {
        Task<object> StartRequest(string action, MessageTypeDefinition messageTypeDefinition, object arguments);
    }
}