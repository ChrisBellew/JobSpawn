using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobSpawn.Common;

namespace JobSpawn.ExampleApp
{
    public class Root : JobSpawnApp
    {
        public ExampleResponse Get(ExampleRequest exampleRequest)
        {
            return new ExampleResponse { ResponseName = exampleRequest.RequestName };
        }
    }

    public class ExampleRequest
    {
        public string RequestName;
    }

    public class ExampleResponse
    {
        public string ResponseName;
    }
}
