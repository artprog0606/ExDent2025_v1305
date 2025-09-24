using AdvantShop.Web.Admin;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SignalRStartup))]
namespace AdvantShop.Web.Admin
{
    public class SignalRStartup
    {
        public void Configuration(IAppBuilder app)
        {
            DisableWebSockets(GlobalHost.DependencyResolver);
            app.MapSignalR(new HubConfiguration
            {
                EnableJavaScriptProxies = false
            });
        }
        
        private static void DisableWebSockets(IDependencyResolver resolver)
        {
            var transportManager = resolver.Resolve<ITransportManager>() as TransportManager;
            transportManager?.Remove("webSockets");
            transportManager?.Remove("serverSentEvents");
            transportManager?.Remove("foreverFrame");
        }
    }
}