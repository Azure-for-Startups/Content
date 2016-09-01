using Microsoft.Owin;
using VideoSharing.Web;

[assembly: OwinStartup(typeof (Startup))]

namespace VideoSharing.Web
{
    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}