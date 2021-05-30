using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MovieReviewPassionProject.Startup))]
namespace MovieReviewPassionProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
