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
        String accessToken { get; set; }
        String url;

        //Get the accesToken for the given AppSecret and AppId
        public void AuthenticateWithFb(String fbAppId, String fbAppSecret)
        {
            String _response=GraphApiGet("oauth/access_token?client_id="+fbAppId+"&client_secret="+fbAppSecret+"&grant_type=client_credentials");
            //String [] split = _response.Split(new char[]{'|'});
            accessToken = _response;
        }

        //GET Request to the Facebook GraphApi
        public String GraphApiGet(String getData)
        {
            String result;
            HttpWebRequest request;
            url = "https://graph.facebook.com/v2.2/";
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

        /**
        * queriing FB API in a grid like fashion to find all pages within a city. this is very intense on the API which is why
        * we shouldn't do this often TODO right now we query 50x50 grid (2500 * (query*paging/query)) queries. So if we have
        * to do paging 3x per query its 7500 calls to the API.. yeah might be obvious what we intend
        * @param city
        */
        //public void FindPagesOfCity(City city)
        //{
        //    double [][]coordinates= city.area;
        //}

        public void shuffle()
        {

        }

        /**
        * returns a 50x50 array with coordinates of the form {lat: Number, lng: Number}
        * @param city
        */
        public List<Coordinates> GetCoordinatesArray(City city){
            int hops = 50;
            List<Coordinates> cityCoordArray= new List<Coordinates>();

            double latHopDist = GetHopDistance(city, "latitude", hops);
            double lngHopDist = GetHopDistance(city, "longitude", hops);

            Coordinates southWest= city.area[1][2];//Southwest

            for(int i=0; i<hops; i++){
                for(int j=0; j<hops;j++){
                    Coordinates coord = new Coordinates(southWest.latitude + latHopDist * i,southWest.longitude + lngHopDist * j);
                    cityCoordArray.Add(coord);
                }
            }

            return cityCoordArray;
        }

        public double GetHopDistance(City city, String angle, int hops)
        {
            //First Coordinate: Southwest, Second: Northeast

            if(angle=="latitude")
                return Math.Abs((city.area[1][2].latitude - city.area[2][1].latitude) / hops);
            else
            {
                return Math.Abs((city.area[1][2].longitude - city.area[2][1].longitude) / hops);
            }
        }


    }
}