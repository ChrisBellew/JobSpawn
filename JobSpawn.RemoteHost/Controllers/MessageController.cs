using System;
using System.Web.Http;
using JobSpawn.Message;

namespace JobSpawn.RemoteHost.Controllers
{
    public class MessageController : ApiController
    {
        public MessageResult Post(Message.Message message)
        {
            var result = Startup.Host.RunMessage(message.Action, message.MessageTypeDefinition, message.Arguments);
            Console.WriteLine("Ran action '{0}' for result '{1}'", message.Action, result);
            return new MessageResult { Result = result };
        }
    }
}