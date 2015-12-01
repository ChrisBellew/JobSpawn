using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace JobSpawn.Client
{
    public class PackageClient
    {
        private HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(new Uri("http://localhost:8080/"), "api/package/");
            return client;
        }

        public void SendPackage(Package.Package package)
        {
            using (var client = CreateClient())
            {
                var task = client.PostAsJsonAsync(client.BaseAddress, package).Result;
            }
        }
    }
}
