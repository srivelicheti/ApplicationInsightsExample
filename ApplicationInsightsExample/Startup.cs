using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ApplicationInsightsExample.Startup))]
namespace ApplicationInsightsExample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
