using placeToBe.Model.Entities;
using placeToBe.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace placeToBe
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            City Koeln=new City();
            Koeln.polygon = new double[5, 2];
            Koeln.polygon[3, 0] = 50.8295269;
            Koeln.polygon[3, 1] = 6.7725819;
            Koeln.polygon[1, 0] = 51.08496299999999;
            Koeln.polygon[1, 1] = 7.1620628;
            FbCrawler fb = new FbCrawler();
            fb.FindPagesForCities(Koeln);
        }
    }
}
