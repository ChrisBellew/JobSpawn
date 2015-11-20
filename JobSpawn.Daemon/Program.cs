using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace JobSpawn.Daemon
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:9000/";

            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.ReadLine();
                //SendPackage(baseAddress, @"G:\Dropbox\Important\Tax");
            }

            Console.ReadLine();
        }

        private static async void SendPackage(string baseAddress, string directory)
        {
            var tempZipFile = Guid.NewGuid().ToString();
            ZipFile.CreateFromDirectory(directory, tempZipFile);

            try
            {
                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        content.Add(new StreamContent(File.OpenRead(tempZipFile)), "package", "package.zip");

                        var response = client.PostAsync(baseAddress + "api/deployment", content).Result;

                        /*using (var message = await client.PostAsync(baseAddress + "api/deployment", new StringContent("")))
                        {
                            var input = await message.Content.ReadAsStringAsync();
                            Console.WriteLine(input);
                        }*/
                    }
                }
            }
            finally
            {
                File.Delete(tempZipFile);
            }
        }
    }
}
