using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using JobSpawn.Common;
using Newtonsoft.Json;

namespace JobSpawn.Daemon
{
    public class DeploymentController : ApiController
    {
        public async void Post()
        {
            var deploymentId = Guid.NewGuid().ToString();
            var deploymentDirectory = Path.Combine("deployments", deploymentId);
            var deploymentTempZipFile = Path.Combine(deploymentDirectory, "deployment.zip");
            Directory.CreateDirectory(deploymentDirectory);

            using (FileStream fileStream = new FileStream(deploymentTempZipFile, FileMode.Create))
            {
                await Request.Content.ReadAsMultipartAsync().Result.Contents[0].CopyToAsync(fileStream);
            }

            ZipFile.ExtractToDirectory(deploymentTempZipFile, deploymentDirectory);
            File.Delete(deploymentTempZipFile);

            var manifest = JsonConvert.DeserializeObject<Package>(File.ReadAllText(Path.Combine(deploymentDirectory, "jobspawn-manifest.json")));
            manifest.DependencyPaths.ToList().ForEach(x => Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), deploymentDirectory, Path.GetFileName(x))));
            var mainAssembly = Assembly.LoadFile(manifest.MainAssembly);
            var type = mainAssembly.GetType(manifest.EntryClass);
            var instance = Activator.CreateInstance(type);
        }

        public class DeploymentResult
        {
            public bool Result;
        }
    }
}