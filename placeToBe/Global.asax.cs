using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using placeToBe.Model.Entities;
using placeToBe.Service;

namespace placeToBe
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            FbCrawler fbCrawler = new FbCrawler();
            City cologne = new City();
            cologne.name = "Cologne, Germany";
            cologne.polygon = new double[5, 2] {
                {51.08496299999999, 6.7725819}, {51.08496299999999, 7.1620628}, {50.8295269, 7.1620628},
                {50.8295269, 6.7725819}, {51.08496299999999, 6.7725819}
            };
            fbCrawler.FindPagesForCities(cologne);
        }
    }
}
