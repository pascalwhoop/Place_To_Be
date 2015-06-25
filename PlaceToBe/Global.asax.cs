using placeToBe.Model.Entities;
using placeToBe.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
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
       
        }


        protected void facebookCrawlerInit()
        {
            FbCrawler fbCrawler = new FbCrawler();

            City berlin = new City();
            berlin.formatted_address = "Berlin, Germany";
            berlin.geometry.bounds.northeast = new Northeast() {
                lat = 52.6754542,
                lng = 13.7611176
            };
            berlin.geometry.bounds.southwest = new Southwest() {
                lat = 52.339629599,
                lng = 13.0891553
            };
            fbCrawler.performCrawlingForCity(berlin);

           /* City munich = new City();
            munich.formatted_address = "Munich, Germany";
            munich.polygon = new double[5, 2]{
                {48.2482197,11.360796}, {48.2482197,11.7228755}, {48.0616018,11.7228755}, {48.0616018, 11.360796}, {48.2482197,11.360796}
            };
            fbCrawler.performCrawlingForCity(berlin);

            City hamburg = new City();
            hamburg.formatted_address = "Hamburg, Germany";
            hamburg.polygon = new double[5, 2]{
                {53.717145,9.732151000}, {53.717145,10.123492}, {53.399999,10.123492}, {53.399999, 9.732151000}, {53.717145,9.732151000}
            };
            fbCrawler.performCrawlingForCity(hamburg);

            City cologne = new City();
            cologne.formatted_address = "Cologne, Germany";
            cologne.polygon = new double[5, 2] {
                {51.08496299999999, 6.7725819}, {51.08496299999999, 7.1620628}, {50.8295269, 7.1620628},
                {50.8295269, 6.7725819}, {51.08496299999999, 6.7725819}
            };
            fbCrawler.performCrawlingForCity(cologne);
            */


        }
    }
}