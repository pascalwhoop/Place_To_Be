using placeToBe.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Helpers;

namespace placeToBe.Services
{
    public class GenderizeService
    {

        public string result = string.Empty;
        public void GetGender(String name)
        {
            HttpWebRequest request;
            string getData = "name=" + name;
            URL = "http://api.genderize.io/?";
            Uri uri = new Uri(URL + getData);
            request = (HttpWebRequest)WebRequest.Create(uri);

            request.Method = "GET";

            //request.ContentType = "application/x-www-form-urlencoded";

            request.AllowAutoRedirect = true;

            UTF8Encoding enc = new UTF8Encoding();



            HttpWebResponse Response;
            try
            {
                using (Response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = Response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            this.result = readStream.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                throw ex;
            }
        }

        public String GetGenderInternal()
        {
            return result;
        }

        public void GenderStat(Event eventGen)
        {

        }

        public string URL { get; set; }
    }
}