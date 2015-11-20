using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JobSpawn.Common
{
    public class SpawnController
    {
        private string baseAddress;
        private string preparePackagePath = "prepare";

        public SpawnController(string baseAddress)
        {
            this.baseAddress = baseAddress;
        }

        public void SendPackage(Package package)
        {
            var tempDirectory = Path.Combine(preparePackagePath, Guid.NewGuid().ToString());
            var tempZipFile = Guid.NewGuid().ToString();

            Directory.CreateDirectory(tempDirectory);
            File.WriteAllText(Path.Combine(tempDirectory, "jobspawn-manifest.json"), JsonConvert.SerializeObject(package));
            package.DependencyPaths.ToList().ForEach(x => File.Copy(x, Path.Combine(tempDirectory, Path.GetFileName(x))));
            ZipFile.CreateFromDirectory(tempDirectory, tempZipFile);
            Directory.Delete(tempDirectory, true);

            try
            {
                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        content.Add(new StreamContent(File.OpenRead(tempZipFile)), "package", "package.zip");
                        var response = client.PostAsync(baseAddress + "api/deployment", content).Result;
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
