using Facebook;
using placeToBe.Model.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace placeToBe.Service
{
    public class FbCrawler
    {
        String fbAppSecret;
        String fbAppId;
        String accessToken;
        String url;

        //Get the accesToken for the given AppSecret and AppId
        public void AuthenticateWithFb(String fbAppId, String fbAppSecret)
        {
            String _response=GraphApiGet("oauth/access_token?client_id="+fbAppId+"&client_secret="+fbAppSecret+"&grant_type=client_credentials");
            String [] split = _response.Split(new char[]{'|'});
            accessToken = split[1];
        }
        //GET
        public String GraphApiGet(String getData)
        {
            String result;
            HttpWebRequest request;
            url = "https://graph.facebook.com/";
            Uri uri = new Uri(url + getData);

            request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.AllowAutoRedirect = true;

            HttpWebResponse Response;
            try
            {
                using (Response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = Response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            //String of the json 
                            return result = readStream.ReadToEnd();

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

        public void FindPagesOfCity(City city)
        {
        }


    }
}