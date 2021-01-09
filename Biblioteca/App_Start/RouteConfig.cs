using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Biblioteca
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "CreateRentByCustomer",
                url: "Rents/Create/c/{customerId}",
                defaults: new { controller = "Rents", action = "CreateByCustomer" }
            );

            routes.MapRoute(
                name: "CreateRentByBook",
                url: "Rents/Create/b/{copyId}",
                defaults: new { controller = "Rents", action = "CreateByBook" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
