using AgileBlazor.Shared.CompServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileBlazor.Server
{
    public class NewServiceClient
    {
        private readonly HttpContext http;

        public NewServiceClient(HttpContext _http) => http = _http;

        public static Shared.CompServices.Service1Client InstanceServiceComp()
        {
            //Teste apontando para serviço de produção
            //#if DEBUG
            //            absoluteUri = absoluteUri.Replace("localhost", "agilework-pc");
            //#endif

            var service = new Shared.CompServices.Service1Client();
            try
            {
                if (service.Endpoint != null && service.Endpoint.Binding != null)
                {
                    service.Endpoint.Binding.CloseTimeout = new TimeSpan(1, 1, 1);
                    service.Endpoint.Binding.OpenTimeout = new TimeSpan(1, 1, 1);
                    service.Endpoint.Binding.SendTimeout = new TimeSpan(1, 1, 1);
                    service.Endpoint.Binding.ReceiveTimeout = new TimeSpan(1, 1, 1);
                }

                if (service.Endpoint.Binding is System.ServiceModel.BasicHttpBinding)
                {
                    var basic = (System.ServiceModel.BasicHttpBinding)service.Endpoint.Binding;
                    if (basic != null)
                    {
                        basic.MaxBufferSize = 2147483647;
                        basic.MaxReceivedMessageSize = 2147483647;
                    }
                }
            }
            catch (Exception)
            {
            }

            return service;
        }

    }
}
