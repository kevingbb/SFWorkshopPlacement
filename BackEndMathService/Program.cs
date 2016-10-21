using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using SharedLibrary;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Communication.FabricTransport.Runtime;

namespace BackEndMathService
{
    public class Program
    {
        // Entry point for the application.
        public static void Main(string[] args)
        {
            ServiceRuntime.RegisterServiceAsync("BackEndMathServiceType", context => new WebHostingService(context, "ServiceEndpoint")).GetAwaiter().GetResult();

            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// A specialized stateless service for hosting ASP.NET Core web apps.
        /// </summary>
        internal sealed class WebHostingService : StatelessService, ICommunicationListener, IMath
        {
            private readonly string _endpointName;

            private IWebHost _webHost;

            public WebHostingService(StatelessServiceContext serviceContext, string endpointName)
                : base(serviceContext)
            {
                _endpointName = endpointName;
            }

            #region StatelessService

            protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
            {
                //return new[] { new ServiceInstanceListener(_ => this) };
                //return new[] { new ServiceInstanceListener(context => new FabricTransportServiceRemotingListener(context, this)) };
                //return new[] { new ServiceInstanceListener(context => new ServiceRemotingListener<IMath>(context, this)) };

                return new[] { new ServiceInstanceListener(context => new FabricTransportServiceRemotingListener(context, this, new FabricTransportListenerSettings()
                                    {
                                        EndpointResourceName = "ServiceEndpoint"
                                    }), "REMOTING"),
                               new ServiceInstanceListener(_ => this, "API")
                };
            }

            #endregion StatelessService

            #region ICommunicationListener

            void ICommunicationListener.Abort()
            {
                _webHost?.Dispose();
            }

            Task ICommunicationListener.CloseAsync(CancellationToken cancellationToken)
            {
                _webHost?.Dispose();

                return Task.FromResult(true);
            }

            Task<string> ICommunicationListener.OpenAsync(CancellationToken cancellationToken)
            {
                // Make sure to use the second endpoint as that is the one for the web host,
                // the first endpoint is a remoting endpoint.
                var endpoint = FabricRuntime.GetActivationContext().GetEndpoint("ServiceEndpoint2");

                string serverUrl = $"{endpoint.Protocol}://{FabricRuntime.GetNodeContext().IPAddressOrFQDN}:{endpoint.Port}";

                _webHost = new WebHostBuilder().UseKestrel()
                                               .UseContentRoot(Directory.GetCurrentDirectory())
                                               .UseStartup<Startup>()
                                               .UseUrls(serverUrl)
                                               .Build();

                _webHost.Start();

                return Task.FromResult(serverUrl);
            }

            #endregion ICommunicationListener

            #region IMath

            public async Task<double> DoubleNumberAsync(double number)
            {
                double numberDoubled = 0;
                numberDoubled = System.Math.Pow(number, 2);

                System.Diagnostics.Trace.WriteLine(System.String.Format("Number Doubled: {0}", numberDoubled));
                return numberDoubled;
            }

            #endregion IMath
        }
    }
}
