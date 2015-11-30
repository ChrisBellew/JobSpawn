using System;
using System.Web.Http;
using JobSpawn.Message;

namespace JobSpawn.RemoteHost.Controllers
{
    public class MessageController : ApiController
    {
        public MessageResult Post(Message.Message message)
        {
            return new MessageResult { Result = 42 };
        }
    }
}