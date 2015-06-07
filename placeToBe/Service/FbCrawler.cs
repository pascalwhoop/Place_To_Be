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
using Microsoft.Ajax.Utilities;

namespace placeToBe.Service
{
    public class FbCrawler
    {
        PageRepository repo = new PageRepository();
        EventRepository repoEvent = new EventRepository();
        String fbAppSecret = "469300d9c3ed9fe6ff4144d025bc1148";
        String fbAppId = "857640940981214";
        String AppGoogleKey = "AIzaSyArx67_z9KxbrVMurzBhS2mzqDhrpz66s0";
        String accessToken { get; set; }
        String url;

        public FbCrawler() {
            accessToken = GraphApiGet("", "FBAppToken");
        }

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
                url = "https://graph.facebook.com/v2.2/" + getData + "?access_token=" + fbAppId + "|" + fbAppSecret;
            }
            else if (condition == "searchPlace")
            {
                //split[0]= latitude, split[1]=longitude, split[2]=distance, split[3]=limit
                String[] split = getData.Split(new string[]{"|"}, StringSplitOptions.None);
                url = "https://graph.facebook.com/v2.2/search?q=\"\"&type=place&center=" + split[0] + "," + split[1] + "&distance=" + split[2] + "&limit=" + split[3] + "&fields=id&access_token=" + fbAppId + "|" + fbAppSecret;
            }
            else if (condition == "nextPage")
            {
                url = getData;
            }
            else if (condition == "searchEvent")
            {
                url = "https://graph.facebook.com/v2.2/" + getData + "/events?limit=2000&fields=id&" + accessToken;
            }
            else if (condition == "searchEventData")
            {
                url = "https://graph.facebook.com/v2.2/" + getData + "/?fields=id,name,description,owner,attending_count,declined_count,maybe_count,start_time,end_time,place&" +accessToken;
            }
            else if (condition == "attendingList")
            {
                url = "https://graph.facebook.com/v2.2/" + getData + "/attending?limit=2000&" + accessToken;
            }
            else if (condition == "FBAppToken") {
                url = "https://graph.facebook.com/oauth/access_token?client_id=" + fbAppId + "&client_secret=" +
                      fbAppSecret + "&grant_type=client_credentials";
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
                return null;
            }
        }


        /*
         * Shuffles an array of type Coordinates
         * */

        public Coordinates[] Shuffle(Coordinates[] o)
        {
            Random _random = new Random();
            int n = o.Length;

            for (int i = 0; i < n; i++)
            {
                //NextDouble -> random number between 0 and 1
                int r = i + (int)(_random.NextDouble() * (n - i));
                Coordinates t = o[r];
                o[r] = o[i];
                o[i] = t;
            }
            return o;
        }

        /**
        * queriing FB API in a grid like fashion to find all pages within a city. this is very intense on the API which is why
        * we shouldn't do this often TODO right now we query 50x50 grid (2500 * (query*paging/query)) queries. So if we have
        * to do paging 3x per query its 7500 calls to the API.. yeah might be obvious what we intend
        * @param city
        */
        public void FindPagesForCities(City city)
        {
            String distance = "2000";
            String limit = "2000";

            //Get all Coordinates of a part of the City
            List<Coordinates> coordListCity = GetCoordinatesArray(city);
            //transform list into array
            Coordinates[] coordArrayCity = coordListCity.ToArray();
            //Shuffle the Array
            Coordinates[] coordArrayCityShuffled = Shuffle(coordArrayCity);

            foreach (Coordinates coord in coordArrayCityShuffled)
            {
                String getData = coord.latitude + "|" + coord.longitude + "|" + distance + "|" + limit;
                getData = getData.Replace(",", ".");
                String place = GraphApiGet(getData, "searchPlace");
                Event eventNew = new Event();
                MergePlacesResponse(place, "nextPage", eventNew);
            }
        }


        //find events on every page in db
        public void FindEventOnPage(String pageId)
        {
            Event eventNew = new Event();
            String response = GraphApiGet(pageId, "searchEvent");
            MergePlacesResponse(response, "searchEvent", eventNew);
        }

        //GEt attending List of an event
        public void FindAttendingList(Event eventNew)
        {

            String response = GraphApiGet(eventNew.fbId, "attendingList");
            MergePlacesResponse(response, "attendingList", eventNew);
        }

        /**
        * this function handles the response from the facebook API query of form
        * /search?q=<query>&type=place&center=<lat,lng>&distance<distance>. We want to make sure we get all the places and
        * facebook uses paging so we got to go ahead and follow through the paging process until there is no more paging
        * @param response
        */
        public void MergePlacesResponse(String response, String condition, Event eventNew)
        {
            List<String> placeIdList = new List<String>();
            List<String> addPlaceIdList = new List<String>();

            //GEt from GraphApi
            addPlaceIdList = HandlePlacesResponse(response, condition);
            //Get NExt Page
            int sizeList = addPlaceIdList.Count;
            if (sizeList > 0) {
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
                    String nextResponse = GraphApiGet(nextPage, "nextPage");
                    addPlaceIdList = HandlePlacesResponse(nextResponse, condition);
                    //Get NExt Page
                    sizeList = addPlaceIdList.Count;
                    if (sizeList > 0)
                    {
                        nextPage = addPlaceIdList[sizeList - 1];
                    }
                    else
                    {
                        nextPage = "UNDEFINED";
                        condition = "searchPlace";
                    }
                }
            }
            
            //Merge lists last time
            placeIdList.AddRange(addPlaceIdList);
            //Paging complete now the next step to get more information with these id's
            HandlePlacesIdArrays(placeIdList.ToArray(), condition, eventNew);
        }
        public List<String> HandlePlacesResponse(String response, String getData)
        {
            //new List for PageId
            List<String> placeIdList = new List<String>();
            if (getData == "attendingList")
            {
                //Convert Json to c# Object facebookPageResults
                FacebookPageResultsAttending facebookPageResults;
                facebookPageResults = JsonConvert.DeserializeObject<FacebookPageResultsAttending>(response);
                //get the data part of the FacebookPageresults which contain the id's
                ResultAttending[] data = facebookPageResults.data;
                //add the id's to list
                foreach (ResultAttending facebookResults in data)
                {
                    placeIdList.Add(facebookResults.id + "," + facebookResults.name + "," + facebookResults.rsvp_status);
                }


                if (facebookPageResults.paging != null && facebookPageResults.paging.next != null)
                {
                    //add the nextPage response at the end of the list, for the next request
                    String next = facebookPageResults.paging.next;
                    placeIdList.Add(next);
                }
            }
            else
            {
                //Convert Json to c# Object facebookPageResults
                FacebookPageResults facebookPageResults;
                facebookPageResults = JsonConvert.DeserializeObject<FacebookPageResults>(response);
                //get the data part of the FacebookPageresults which contain the id's
                FacebookResults[] data = facebookPageResults.data;
                //add the id's to list
                if (data != null) {
                    foreach (FacebookResults facebookResults in data)
                    {
                        placeIdList.Add(facebookResults.id);
                    }
                }


                if (facebookPageResults.paging != null && facebookPageResults.paging.next != null)
                {
                    //add the nextPage response at the end of the list, for the next request
                    String next = facebookPageResults.paging.next;
                    placeIdList.Add(next);
                }
            }

            return placeIdList;

        }


        public async Task<Page> PageSearchDb(String fbId)
        {
            Page page = await repo.GetByIdAsync(fbId);
            return page;
        }

        public async Task<Event> EventSearchDb(String fbId)
        {
            Event eventNew = await repoEvent.GetByFbIdAsync(fbId);
            return eventNew;
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

            double southWestLat = city.polygon[3,0];//SouthwestLatitude
            double southWestLng= city.polygon[3,1];//SouthwestLongitude

            for (int i = 0; i < hops; i++)
            {
                for (int j = 0; j < hops; j++)
                {
                    Coordinates coord = new Coordinates(southWestLat + latHopDist * i, southWestLng + lngHopDist * j);
                    cityCoordArray.Add(coord);
                }
            }

            return cityCoordArray;
        }

        /**
         * handles an array of placeIDs and gets the full information for each of them from the FB API
         * @param arr
         */
        public async void HandlePlacesIdArrays(String[] placesId, String condition, Event eventNewZ)
        {
            if (condition == "searchPlace")
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

                    if (page == null) {
                        //Get page information
                        String pageData = GraphApiGet(id, "pageData");
                        //handle the page and push it to db
                        HandlePlace(pageData, condition, "");
                    }
                    else {
                        String pageData = JsonConvert.SerializeObject(page);
                        HandlePlace(pageData, condition, "");

                    }
                }
            }
            else if (condition == "searchEvent")
            {
                Event eventNew;
                foreach (String id in placesId)
                {
                    try
                    {
                        eventNew = await EventSearchDb(id);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception: " + e);
                        eventNew = null;

                    }

                    if (eventNew == null)
                    {
                        //Get Event information
                        String eventData = GraphApiGet(id, "searchEventData");
                        //handle the Event and push it to db
                        HandlePlace(eventData, condition, id);
                    }
                }

            }
            else if (condition == "attendingList")
            {
                HandleAttendingList(placesId, eventNewZ);
            }
        }

        //split data in right place
        public void HandleAttendingList(String[] peopleId, Event eventNew)
        {//facebookResults.id+","+facebookResults.name+","+facebookResults.rsvp_status);

            List<Rsvp> list = new List<Rsvp>();
            foreach (String rsvp in peopleId)
            {
                char splitChar = ',';
                String[] rsvpSplit = rsvp.Split(splitChar);
                Rsvp eventRsvp = new Rsvp();
                eventRsvp.id = rsvpSplit[0];
                eventRsvp.name = rsvpSplit[1];
                eventRsvp.rsvp_status = rsvpSplit[2];
                list.Add(eventRsvp);
            }
            PushEventToDb(eventNew, list);
        }

        /**
        * handles a single FB place and saves it in the DB
        * @param place
        * @param callback
        */
        public void HandlePlace(String place, String condition, String id) {
            if (place == null) return;
            //Place
            if (condition == "searchPlace")
            {
                JObject _place = JObject.Parse(place);
                try {
                    var isCommunityPage = (bool) _place["is_community_page"];
                    JToken token = _place["GeoLocation"];
                    if (isCommunityPage == false) {
                        //we only save non-community-pages since only they will actually create events
                        //Convert json to Object
                        Page page = JsonConvert.DeserializeObject<Page>(place);
                        //a place that is not community owned is == to a page in the facebook world
                        //insert in db
                        PushToDb(page);
                        FindEventOnPage(page.fbId);
                    }
                    else if (isCommunityPage == true) {
                        //CommunityPage save?
                    }
                }
                catch (ArgumentNullException ex) {
                    Console.WriteLine("Exception: " + ex);
                }
            }
            //Event
            else if (condition == "searchEvent")
            {
                
                Event eventNew = JsonConvert.DeserializeObject<Event>(place);
                FindAttendingList(eventNew);
            }
        }

        //Insert page to Db
        public async void PushToDb(Page page)
        {
            await repo.InsertAsync(page);
        }

        //Inster event to db
        public async void PushEventToDb(Event eventNew, List<Rsvp> list)
        {
            eventNew.attending = list;
            eventNew=FillEmptyEventFields(eventNew);
            if (eventNew != null) {
                await repoEvent.InsertAsync(eventNew);                
            }
        }

        public double GetHopDistance(City city, String angle, int hops)
        {
            //First Coordinate: Southwest, Second: Northeast

            if (angle == "latitude")
                return Math.Abs((city.polygon[3, 0] - city.polygon[1, 0]) / hops);
            else
            {
                return Math.Abs((city.polygon[3, 1] - city.polygon[1, 1]) / hops);
            }
        }
        /// <summary>
        /// Adds location data (latitude, longitude) to an Event
        /// </summary>
        /// <param name="entity">Event modified with location data</param>
        /// <returns>modified Event if sucsessful, otherwise just the original Event</returns>
        public Event FillEmptyEventFields(Event e)

        {
            try
            {


                if (e.place != null && e.place.location != null && e.place.location.latitude != 0 && e.place.location.longitude != 0) {
                    e.geoLocationCoordinates = new GeoLocation(e.place.location.latitude, e.place.location.longitude);
                }
                else if (e.place != null && e.place.name != null)
                {

                    String getData = e.place.name;
                            //Needs to be more defined (first exsample was: name = Alexanderplatz, Berlin)
                    
                    JObject obj = JObject.Parse(GraphApiGet(getData, "GOOGLE"));
                    Array arr = (Array) obj["Results"];
                    if (obj["results"] != null && arr != null && arr.Length != 0) {
                        double lat = Convert.ToDouble(obj["results"][0]["geometry"]["location"]["lat"]);
                        double lng = Convert.ToDouble(obj["results"][0]["geometry"]["location"]["lng"]);
                        e.geoLocationCoordinates = new GeoLocation(lat, lng);
                    }
                    
                }else
                {
                    return null;
                }
            }
            catch (NullReferenceException ex)
            {
                Console.Write(ex.ToString());
            }

            e = fillStartEndDateTime(e);
            return e;
        }

        //we add converted fields to the object which we can later put an index on
        public Event fillStartEndDateTime(Event e) {
            if (e.start_time != null) {
                e.startDateTime = UtilService.getDateTimeFromISOString(e.start_time);
            }
            if (e.end_time != null) {
                e.endDateTime = UtilService.getDateTimeFromISOString(e.end_time);
            }
            return e;
        }
    }
}