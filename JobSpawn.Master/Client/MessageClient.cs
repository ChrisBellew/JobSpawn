using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JobSpawn.Message;

namespace JobSpawn.Client
{
    public class MessageClient
    {
        private HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(new Uri("http://localhost:8080/"), "api/message/");
            return client;
        }

        public Task<MessageResult> SendMessage(Message.Message message)
        {
            HttpResponseMessage response;
            using (var client = CreateClient())
            {
                response = client.PostAsJsonAsync(client.BaseAddress, message).Result;
            }
            return response.Content.ReadAsAsync<MessageResult>();
        }
    }
}
