using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TZLightIt.Startup))]
namespace TZLightIt
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
