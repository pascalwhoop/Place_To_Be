using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace placeToBe
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes(); // attribute routing http://www.asp.net/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2

            //convention based routing
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
