using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using System.Fabric;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using SharedLibrary;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace FrontEndWebServer.Controllers
{
    public class MathController : Controller
    {
        private static readonly Uri backEndServiceUri;
        private static HttpClient client = new HttpClient();

        static MathController()
        {
            backEndServiceUri = new Uri(FabricRuntime.GetActivationContext().ApplicationName + "/BackEndMathService");

            // This URL is to connect to the internal load balancer on the
            // SF Cluster BackEnd nodetype that is listening on a static port.
            // The reason it is an IP Address is becuase there is no DNS name
            // resolution for parts of the cluster that are not exposed internally.
            // That is why the recommendation is to use Reverse Proxy or the
            // Naming Service with Remoting.
            //TODO: This will have to be updated before the API call will work.
            client.BaseAddress = new Uri("http://10.0.2.5:25001");

            // This URL is to connect to the reverse proxy on the SF Cluster
            // BackEnd nodetype listening on the designated Reverse Proxy port
            // per the SF CLuster manifest, which is defined in the ARM Template.
            // The default Reverse Proxy port is usually 19008.
            //TODO: This will have to be updated before the API call will work.
            //client.BaseAddress = new Uri("http://localhost:19008/SFWorkshopPlacement/BackEndMathService/");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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

        public async Task<IActionResult> DoubledViaAPI(double number, string backendurl)
        {
            string numberDoubled = String.Empty;

            // Call the BackEndMathService, which is a Stateless Service, via
            // an api versus remoting.

            // You can use the static reference or the dynamic one that is passed
            // in. Recommend the dynamic one for demoing and troubleshooting.
            //HttpResponseMessage response = await client.GetAsync(String.Format("api/doubles/{0}", number.ToString()));

            HttpClient backendClient = new HttpClient();
            backendClient.BaseAddress = new Uri(backendurl);
            backendClient.DefaultRequestHeaders.Accept.Clear();
            backendClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await backendClient.GetAsync(String.Format("api/doubles/{0}", number.ToString()));

            if (response.IsSuccessStatusCode)
            {
                numberDoubled = await response.Content.ReadAsStringAsync();
            }

            ViewData["Doubled"] = numberDoubled;

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
