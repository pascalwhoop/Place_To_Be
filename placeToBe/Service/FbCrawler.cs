using Newtonsoft.Json.Linq;
using placeToBe.Model;
using placeToBe.Model.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using placeToBe.Model.Repositories;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace placeToBe.Service
{
    public class FbCrawler
    {
        PageRepository repo = new PageRepository();
        String fbAppSecret = "469300d9c3ed9fe6ff4144d025bc1148";
        String fbAppId = "857640940981214";
        String AppGoogleKey = "AIzaSyArx67_z9KxbrVMurzBhS2mzqDhrpz66s0";
        String accessToken { get; set; }
        String url;

        ////Get the accesToken for the given AppSecret and AppId
        //public void AuthenticateWithFb(String fbAppId, String fbAppSecret)
        //{
        //    String _response=GraphApiGet("oauth/access_token?client_id="+fbAppId+"&client_secret="+fbAppSecret+"&grant_type=client_credentials");
        //    //String [] split = _response.Split(new char[]{'|'});
        //    accessToken = _response;
        //}

        //GET Request to the Facebook GraphApi
        public String GraphApiGet(String getData, String condition)
        {
            String result;
            HttpWebRequest request;

            if (condition == "GOOGLE")
            {
                url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + getData + "&key=" + AppGoogleKey;
            }
            else if (condition == "pageData")
            {
                url = "https://graph.facebook.com/v2.2/" + getData + "&access_token=" + fbAppId + "|" + fbAppSecret;
            }
            else if (condition == "searchPlace")
            {
                String[] split = getData.Split(new char['|']);
                url = "https://graph.facebook.com/v2.2/search?q=\"\"&type=place&center=" + split[0] + "," + split[1] + "&distance=" + split[2] + "&limit=" + split[3] + "&fields=id&access_token=" + fbAppId + "|" + fbAppSecret;
            }
            else if (condition == "nextPage")
            {
                url = getData;
            }

            Uri uri = new Uri(url);

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



        public Coordinates[] shuffle(Coordinates[] o)
        {
            Random _random = new Random();
            int n = o.Length;

            for (int i = 0; i < n; i++)
            {
                //NextDouble() gibt eine Zufallszahl zwischen 0 und 1 wie Math.Random() RR-RANDOOM Java.
                int r = i + (int)(_random.NextDouble() * (n - i));
                Coordinates t = o[r];
                o[r] = o[i];
                o[i] = t;
            }
            return o;
        }

        public void FindPagesForCities(City city)
        {
            String distance = "2000";
            String limit = "5000";

            //Get all Coordinates of a part of the City
            List<Coordinates> coordListCity = GetCoordinatesArray(city);
            //transform list into array
            Coordinates[] coordArrayCity = coordListCity.ToArray();
            //Shuffle the Array
            Coordinates[] coordArrayCityShuffled = shuffle(coordArrayCity);

            foreach (Coordinates coord in coordArrayCityShuffled)
            {
                String getData = coord.latitude + "|" + coord.longitude + "|" + distance + "|" + limit;
                String place = GraphApiGet(getData, "searchPlace");
                MergePlacesResponse(place);
            }
        }

        /**
        * this function handles the response from the facebook API query of form
        * /search?q=<query>&type=place&center=<lat,lng>&distance<distance>. We want to make sure we get all the places and
        * facebook uses paging so we got to go ahead and follow through the paging process until there is no more paging
        * @param response
        */

        public void MergePlacesResponse(String response)
        {
            List<String> placeIdList = new List<String>();
            List<String> addPlaceIdList = new List<String>();
            
            //GEt from GraphApi
            addPlaceIdList=HandlePlacesResponse(response);
            //Get NExt Page
            int sizeList = addPlaceIdList.Count;
            String nextPage = addPlaceIdList[sizeList - 1];



            //Search for more Pages until the end
            while (nextPage.Substring(0, 5) == "https")
            {
                //Delete Page from id List
                addPlaceIdList.RemoveAt(sizeList - 1);
                //Merge lists
                placeIdList.AddRange(addPlaceIdList);
                //clear addPlaceIdList
                addPlaceIdList.Clear();
                //Get from Graph APi
                addPlaceIdList = HandlePlacesResponse(response);
                //Get NExt Page
                sizeList = addPlaceIdList.Count;
                nextPage = addPlaceIdList[sizeList - 1];
            }
            //Paging complete now the next step to get more information with these id's
            HandlePlacesIdArrays(placeIdList.ToArray());
        }
        public List<String> HandlePlacesResponse(String response)
        {
            //new List for PageId
            List<String> placeIdList = new List<String>();

            //Convert Json to c# Object facebookPageResults
            FacebookPageResults facebookPageResults;
            facebookPageResults = JsonConvert.DeserializeObject<FacebookPageResults>(response);
            //get the data part of the FacebookPageresults which contain the id's
            FacebookResults[] data = facebookPageResults.data;
            //add the id's to list
            foreach (FacebookResults facebookResults in data)
            {
                placeIdList.Add(facebookResults.id);
            }

            try
            {
                //add the nextPage response at the end of the list, for the next request
                String next = facebookPageResults.paging.next;
                String nextResponse = GraphApiGet(next, "nextPage");
                placeIdList.Add(nextResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine("Page end, there's no next page!");
            }
            return placeIdList;

        }


        public async Task<Page> PageSearchDb(String fbId)
        {
            Page page = await repo.GetByIdAsync(fbId);
            return page;
        }

        /**
        * returns a 50x50 array with coordinates of the form {lat: Number, lng: Number}
        * @param city
        */
        public List<Coordinates> GetCoordinatesArray(City city)
        {
            int hops = 50;
            List<Coordinates> cityCoordArray = new List<Coordinates>();

            double latHopDist = GetHopDistance(city, "latitude", hops);
            double lngHopDist = GetHopDistance(city, "longitude", hops);

            Coordinates southWest = city.area[1][2];//Southwest

            for (int i = 0; i < hops; i++)
            {
                for (int j = 0; j < hops; j++)
                {
                    Coordinates coord = new Coordinates(southWest.latitude + latHopDist * i, southWest.longitude + lngHopDist * j);
                    cityCoordArray.Add(coord);
                }
            }

            return cityCoordArray;
        }

        /**
         * handles an array of placeIDs and gets the full information for each of them from the FB API
         * @param arr
         */
        public async void HandlePlacesIdArrays(String[] placesId)
        {
            Page page;
            foreach (String id in placesId)
            {
                try
                {
                    page = await PageSearchDb(id);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e);
                    page = null;

                }

                if (page == null)
                {
                    //Get page information
                    String pageData = GraphApiGet(id, "pageData");
                    //handle the page and push it to db
                    HandlePlace(pageData);
                }
            }
        }

        /**
        * handles a single FB place and saves it in the DB
        * @param place
        * @param callback
        */
        public void HandlePlace(String place)
        {
            JObject _place = JObject.Parse(place);
            String isCommunityPage = (String)_place["is_community_page"];
            JToken token = _place["Location"];
            if (isCommunityPage == "false")
            {
                //we only save non-community-pages since only they will actually create events
                Page page = new Page();
                //Convert json to Object
                page = JsonConvert.DeserializeObject<Page>(place);
                //a place that is not community owned is == to a page in the facebook world
                //insert in db
                PushToDb(page);
            }
            else if (isCommunityPage == "true")
            {
                //CommunityPage save?
            }
        }

        public async void PushToDb(Page page)
        {
            await repo.InsertAsync(page);
        }

        public double GetHopDistance(City city, String angle, int hops)
        {
            //First Coordinate: Southwest, Second: Northeast

            if (angle == "latitude")
                return Math.Abs((city.area[1][2].latitude - city.area[2][1].latitude) / hops);
            else
            {
                return Math.Abs((city.area[1][2].longitude - city.area[2][1].longitude) / hops);
            }
        }

        public Event fillEmptyEventFields(Event e)
        {
            //Setting location of an Event in right dataformat for geospital Index

            e.locationCoordinates.coordinates = new double[2];

            if (e.venue.latitude != 0 && e.venue.longitude != 0)
            {

                e.locationCoordinates.coordinates[0] = e.venue.latitude;
                e.locationCoordinates.coordinates[1] = e.venue.longitude;
            }
            else
            {

                String getData = "";

                if (e.venue.name != null)
                {
                    getData += "e.venue.name"; //Needs to be more defined (first exsample was: name = Alexanderplatz, Berlin)
                }

                JObject googleLocation = JObject.Parse(GraphApiGet(getData, "GOOGLE"));
                try
                {
                    e.locationCoordinates.coordinates[0] = Convert.ToDouble(googleLocation.SelectToken("results[1].geometry.location.lat").ToString());
                    e.locationCoordinates.coordinates[1] = Convert.ToDouble(googleLocation.SelectToken("results[1].geometry.location.lng").ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Google wasn't able to find coordinates of: {0}", getData);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return e;
        }
    }
}