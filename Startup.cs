using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Barshop.Startup))]
namespace Barshop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
