using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ServiceFabric.Services.Remoting;

namespace SharedLibrary
{
    public interface IMath : IService
    {
        Task<double> DoubleNumberAsync(double number);
    }
}
