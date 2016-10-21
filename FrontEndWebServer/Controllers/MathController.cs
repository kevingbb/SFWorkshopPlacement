using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using System.Fabric;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using SharedLibrary;

namespace FrontEndWebServer.Controllers
{
    public class MathController : Controller
    {
        private static readonly Uri backEndServiceUri;

        static MathController()
        {
            backEndServiceUri = new Uri(FabricRuntime.GetActivationContext().ApplicationName + "/BackEndMathService");
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Doubled(double number)
        {
            double numberDoubled = 0;

            // Call the BackEndMathService which is a Stateless Service which
            // is why there are no partition parameters. The reason for the 
            // listener name is because there is more than one listener endpoint.
            // One endpoint is a remoting endpoing and the other is an api endpoint.
            IMath math = ServiceProxy.Create<IMath>(backEndServiceUri, listenerName: "REMOTING");
            numberDoubled = await math.DoubleNumberAsync(number);

            ViewData["Doubled"] = numberDoubled;

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
