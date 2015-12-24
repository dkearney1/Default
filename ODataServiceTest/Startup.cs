using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ODataServiceTest.Startup))]
namespace ODataServiceTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
