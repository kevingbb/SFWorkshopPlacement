using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using System.Fabric;
using SharedLibrary;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace BackEndMathService.Controllers
{
    [Route("api/[controller]")]
    public class DoublesController : Controller
    {
        private static readonly Uri backEndServiceUri;

        static DoublesController()
        {
            backEndServiceUri = new Uri(FabricRuntime.GetActivationContext().ApplicationName + "/BackEndMathService");
        }

        // GET api/doubles
        [HttpGet]
        public IEnumerable<string> Get()
        {
            string NoNumber = "No Number Provided.";
            System.Diagnostics.Trace.WriteLine(NoNumber);
            return new string[] { NoNumber };
        }

        // GET api/doubles/5
        [HttpGet("{number}")]
        public async Task<IEnumerable<string>> Get(double number)
        {
            double numberDoubled = 0;
 
            // Call the BackEndMathService which is a Stateless Service which
            // is why there are no partition parameters. The reason for the 
            // listener name is because there is more than one listener endpoint.
            // One endpoint is a remoting endpoing and the other is an api endpoint.
            IMath math = ServiceProxy.Create<IMath>(backEndServiceUri, listenerName: "REMOTING");
            numberDoubled = await math.DoubleNumberAsync(number);

            System.Diagnostics.Trace.WriteLine(String.Format("Number Doubled: {0}", numberDoubled));
            return new string[] { numberDoubled.ToString() };
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
