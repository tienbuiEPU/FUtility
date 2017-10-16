using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FUtility.Startup))]
namespace FUtility
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
