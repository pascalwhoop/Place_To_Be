using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json.Linq;
using placeToBe.Model.Entities;
using placeToBe.Services;

namespace placeToBe
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start() {

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Thread backGender;
            //backGender = new Thread(GenderizeInit);
            //backGender.IsBackground = true;
            //backGender.Start();

        }

        protected async void GenderizeInit()
        {
            GenderizeService genderService= new GenderizeService();

            StreamReader streamReader= new StreamReader("C:/Repos/Place_To_Be/placeToBe.Tests/Ressources/names2.json");
            String json = streamReader.ReadToEnd();
            JToken resultObjects = JToken.Parse(json);
            List<String> array = new List<string>();
            String[] splittedArray;
            int i = 0;
            foreach (JToken token in resultObjects)
            {
                splittedArray = token.ToString().Split(new[]{" ","-"},StringSplitOptions.None);
                array.Add(splittedArray[0]);

            }

            Debug.WriteLine("########## Start Gendering ########");
            Gender[] genderArray = new Gender[array.Count];
            int j = 0;
            foreach (String element in array)
            {
                Debug.WriteLine("########GETTING GENDER FOR "+element+" ############");
                genderArray[j]=await genderService.GetGender(element);
                Debug.WriteLine("######Gender: " + genderArray[j].gender+" ######## "+(j+1));
                j++;
            }

            Debug.WriteLine("######## Finished ######");
            
        }

    }
}