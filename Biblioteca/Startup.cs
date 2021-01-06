using Biblioteca.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Biblioteca.Startup))]
namespace Biblioteca
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateUserRoles();
        }

        private void CreateUserRoles()
        {
            var ctx = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ctx));

            if(!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole("Admin");
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Customer"))
            {
                var role = new IdentityRole("Customer");
                roleManager.Create(role);
            }
        }
    }
}
