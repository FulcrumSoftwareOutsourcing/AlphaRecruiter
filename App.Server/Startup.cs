using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(App.Server.Startup))]
namespace App.Server
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
