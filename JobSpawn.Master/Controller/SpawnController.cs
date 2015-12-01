using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using JobSpawn.Client;
using JobSpawn.Host;
using JobSpawn.Message;
using JobSpawn.Package;

namespace JobSpawn.Controller
{
    public class SpawnController : ISpawnController
    {
        private readonly Type concreteType;
        private readonly IHost host;

        public SpawnController(Type concreteType)
        {
            this.concreteType = concreteType;

            var package = new Package.Package();
            package.EntryAssembly = concreteType.Assembly.GetName().Name + ".dll";
            package.EntryType = concreteType.FullName;
            package.Assemblies = concreteType.Assembly.GetReferencedAssemblies().Concat(new [] { concreteType.Assembly.GetName() }).Select(x => new PackageAssembly { AssemblyName = x.Name + ".dll", AssemblyBytes = File.ReadAllBytes(GetAssemblyLocation(x)) }).ToList();

            var packageClient = new PackageClient();
            packageClient.SendPackage(package);

            //host = hostBuilder.BuildHost(concreteType);
        }

        public async Task<object> StartRequest(string action, MessageTypeDefinition messageTypeDefinition, object arguments)
        {
            var message = new Message.Message { Action = action, MessageTypeDefinition = messageTypeDefinition, Arguments = arguments };

            MessageClient messageClient = new MessageClient();
            var messageResult = await messageClient.SendMessage(message);

            var returnType = concreteType.GetMethod(action).ReturnType;
            //var obj = ByteArrayToObject(messageResult.Result);

            return Convert.ChangeType(messageResult.Result, returnType);

            //var result = host.RunMessage(action, messageTypeDefinition, messageBytes);
            //var result =  messageResult.Result;
        }

        public static object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();

            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);

            object obj = (object)binForm.Deserialize(memStream);

            return obj;
        }

        private string GetAssemblyLocation(AssemblyName assemblyName)
        {
            return Assembly.ReflectionOnlyLoad(assemblyName.FullName).Location;
        }
    }
}
