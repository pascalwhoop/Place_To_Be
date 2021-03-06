using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using placeToBe.Model;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;

namespace placeToBe.Services {
    public class FbCrawler {
        private PageRepository pageRepo = new PageRepository();
        private readonly EventRepository eventRepo = new EventRepository();
        GenderizeService genderizeService=new GenderizeService();
        private readonly String fbAppSecret = ConfigurationManager.AppSettings["fbAppSecret"];
        private readonly String fbAppId = ConfigurationManager.AppSettings["fbAppId"];
        private readonly String appGoogleKey = ConfigurationManager.AppSettings["googleAppKey"];
        private String accessToken { get; set; }
        private String url;

    
        ///
        /// <summary>constructor. get the fb acces token to use the fb api and store it. in case it doesnt work we retry 20 times</summary>
        /// 
        public FbCrawler() {
            accessToken = graphApiGet("", "FBAppToken");
            if (accessToken == "") {
                Debug.WriteLine("didnt receive token. retrying... ");

                for (var i = 0; i < 20; i++) {
                    Debug.Write(".");
                    Thread.Sleep(1000);
                    accessToken = graphApiGet("", "FBAppToken");

                    if (accessToken != "")
                    {
                        break;
                    }
                }
            }
        }


        //GET 
        
        /// <summary>
        /// Helper method to send GET requests to the WEB APIs we use.
        /// </summary>
        /// <param name="getData">a string that contains the parameters for the specified request</param>
        /// <param name="requestToPerform">a string describing the request to perform. e.g. GOOGLE for google maps geocode,...</param>
        /// <returns></returns>
        public String graphApiGet(String getData, String requestToPerform) //synchrone Operation
        {
            if (requestToPerform == "GOOGLE")
                url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + getData + "&key=" + appGoogleKey;
            else {
                switch (requestToPerform) {
                    case "pageData":
                        url = "https://graph.facebook.com/v2.3/" + getData + "?access_token=" + fbAppId + "|" +
                              fbAppSecret;
                        break;
                    case "searchPlace":
                        //split[0]= latitude, split[1]=longitude, split[2]=distance, split[3]=limit
                        var split = getData.Split(new[] {"|"}, StringSplitOptions.None);
                        url = "https://graph.facebook.com/v2.3/search?q=&type=place&center=" + split[0] + "," + split[1] +
                              "&distance=" + split[2] + "&limit=" + split[3] +
                              "&fields=id,is_community_page&access_token=" + fbAppId + "|" + fbAppSecret;
                        break;
                    case "nextPage":
                        url = getData;
                        break;
                    case "searchEvent":
                        url = "https://graph.facebook.com/v2.3/" + getData +
                              "/events?limit=2000&fields=id,attending_count,start_time&" + accessToken;
                        break;
                    case "searchEventData":
                        url = "https://graph.facebook.com/v2.3/" + getData +
                              "/?fields=id,name,description,owner,attending_count,declined_count,maybe_count,start_time,end_time,place,cover&" +
                              accessToken;
                        break;
                    case "attendingList":
                        url = "https://graph.facebook.com/v2.3/" + getData + "/attending?limit=2000&" + accessToken;
                        break;
                    case "FBAppToken":
                        url = "https://graph.facebook.com/oauth/access_token?client_id=" + fbAppId + "&client_secret=" +
                              fbAppSecret + "&grant_type=client_credentials";
                        break;
                }
            }

            var uri = new Uri(url);

            Debug.WriteLine("GET: " + requestToPerform);
            return UtilService.performGetRequest(uri);
        }



        /// <summary>
        /// Shuffles an array
        /// </summary>
        /// <typeparam name="T">the object type of the objects in the array</typeparam>
        /// <param name="o">the array to shuffle</param>
        /// <returns></returns>
        public T[] shuffle<T>(T[] o) {
            var random = new Random();
            var n = o.Length;

            for (var i = 0; i < n; i++) {
                //NextDouble -> random number between 0 and 1
                var r = i + (int) (random.NextDouble()*(n - i));
                var t = o[r];
                o[r] = o[i];
                o[i] = t;
            }
            return o;
        }

       

        /// <summary>
        /// Querying FB API in a grid like fashion to find all pages within a city. this is very intense on the API which is why
        /// we shouldn't do this often (or too fast).
        ///
        /// </summary>
        /// <param name="city">the city to crawl over</param>
        public async void performCrawlingForCity(City city) {
            var distance = "2000";
            var limit = "2000";

            //Get all Coordinates of a part of the City
            var coordListCity = createCoordinatesArray(city);
            //transform list into array
            var coordArrayCity = coordListCity.ToArray();
            //Shuffle the Array
            var coordArrayCityShuffled = shuffle(coordArrayCity);
             
            Debug.WriteLine("WILL FETCH ALL EVENTS FOR "+ city.formatted_address + " . OUR SEARCH GRID HOLDS " + coordArrayCityShuffled.Length +
                            " BLOCKS");
            Debug.WriteLine(
                "==================================================================================================");

            for (var i=0;i<coordArrayCityShuffled.Length/3;i++) { //we only fetch one half of the search grid. usually after having fetched 25% of it, most events are already found and also most are already up to date. very few will be missed and those will likely be updated the next time
                var coord = coordArrayCityShuffled[i];
                var getData = coord.latitude + "|" + coord.longitude + "|" + distance + "|" + limit;
                getData = getData.Replace(",", ".");
                Debug.WriteLine("FETCHING BLOCK: " + i);
                Debug.WriteLine("COORDINATES: " + coord.latitude + " --- " + coord.longitude);
                Debug.WriteLine("City: " + city.formatted_address);
                Debug.WriteLine("===========================================");
                //fetch all Places for this coordinate
                var pages =
                    completePaging(
                        JsonConvert.DeserializeObject<FacebookPageResults>(graphApiGet(getData, "searchPlace")));
                //and for each of them find all events if they arent a community page
                await handlePagesList(pages);
            }
        }

        
        /// <summary>
        ///  we now have a list of {id: ..., is_community_page: true/false} .. we process this list. all community pages dont get called, for the others we fetch the details and the events
        /// </summary>
        /// <param name="pages">a list of FacebookPagingResult objects that describe the FBPages that we want to crawl</param>
        /// <returns>nothing. but it's async.</returns>
        public async Task handlePagesList(List<FacebookPagingResult> pages) {            
            foreach (var page in pages) {
                if (!page.is_community_page) {
                    try {
                        var pageInDb = pageRepo.GetByFbIdAsync(page.fbId).Result;

                        if (pageInDb == null) {
                            var t = fetchAndStorePage(page.fbId); //only fetching events that arent in the DB yet.
                            var p = t.Result;
                            //for now we dont fetch events here anymore. we just get all the pages and place them in the DB. Another crawling task will take care of all the places and their events and fetch them.
                            /*if (determineIfEventsShouldBeFetched(p))
                                fetchEventsOnPage(p.fbId);*/
                        }
                        
                    }
                    catch (Exception exception) {
                        Debug.WriteLine(exception); // TODO VERY DIRTY FIX ASAP
                    }
                }
            }
        }

        /// <summary>
        /// check if we should check the pages Events (only if page update is over 1 day ago or under 5 seconds ago)
        /// </summary>
        /// <param name="p">the page in question</param>
        /// <returns>a boolean telling us to fetch data from the page or not</returns>
        
        /*private static bool determineIfEventsShouldBeFetched(Page p) {
            bool shouldFetchEvents;
            var pLastChecked = p.lastUpdatedTimestamp;
            var now = DateTime.Now;
            var timeDiff = now - pLastChecked;
            if (timeDiff < TimeSpan.FromSeconds(5) || timeDiff > TimeSpan.FromHours(24)) shouldFetchEvents = true;
            else shouldFetchEvents = false;
            return shouldFetchEvents;
        }*/

       /// <summary>
       /// fetch all events on a page. We first fetch a list of events, then get all the details for every page
       /// </summary>
       /// <param name="pageId">the fbid for the page to fetch the events for</param>
       /// <returns>a task that can be awaited and then a number of events fetched</returns>
        public async Task fetchEventsOnPage(String pageId) {
           var page = await pageRepo.GetByFbIdAsync(pageId);
            var response = graphApiGet(pageId, "searchEvent");
            var results =
                completePaging(JsonConvert.DeserializeObject<FacebookPageResults>(response));
            foreach (var e in results) {
                if (e.attending_count > 5 && DateTime.Now < UtilService.getDateTimeFromISOString(e.start_time)) {
                    try {
                        //if event fetched and was new one increase eventCount by one
                        if(await fetchAndStoreEvent(e.fbId)) page.eventCount++;
                    }
                    catch (Exception ex) {
                        Debug.WriteLine(ex.StackTrace);
                    }
                }
            }
           pageRepo.UpdateAsync(page); //safe the page which now has a hhigher eventCount

       }

        /// <summary>
        /// GEt attending List of an event
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public List<Rsvp> fetchAttendingList(String eventId) {
            var response = graphApiGet(eventId, "attendingList");

            var r = completePaging(JsonConvert.DeserializeObject<FacebookPageResults>(response));

            return makeAttendingList(r);
        }

        /**
        * this function handles the response from the facebook API query of form
        * /search?q=<query>&type=place&center=<lat,lng>&distance<distance>. We want to make sure we get all the places and
        * facebook uses paging so we got to go ahead and follow through the paging process until there is no more paging
        * @param response
        */

        public List<FacebookPagingResult> completePaging(FacebookPageResults fbApiPagingResponse) {
            var pagingResults = new List<FacebookPagingResult>();
            pagingResults.AddRange(fbApiPagingResponse.data);

            while (fbApiPagingResponse.paging != null && fbApiPagingResponse.paging.next != null) {
                //and fetch next
                fbApiPagingResponse =
                    JsonConvert.DeserializeObject<FacebookPageResults>(graphApiGet(fbApiPagingResponse.paging.next,
                        "nextPage"));
                //put fetched page into our collecting list
                pagingResults.AddRange(fbApiPagingResponse.data);
            }
            return pagingResults;
        }


        public async Task<Event> eventSearchByFbId(String fbId) {
            return await eventRepo.GetByFbIdAsync(fbId);
        }

        /**
        * returns a 50x50 array with coordinates of the form {lat: Number, lng: Number}
        * @param city
        */

        public List<Coordinates> createCoordinatesArray(City city) {
            const int hops = 100;
            var cityCoordArray = new List<Coordinates>();

            var latHopDist = getHopDistance(city, "latitude", hops);
            var lngHopDist = getHopDistance(city, "longitude", hops);

            var southWestLat = city.getPolygon()[3, 0]; //SouthwestLatitude
            var southWestLng = city.getPolygon()[3, 1]; //SouthwestLongitude

            for (var i = 0; i < hops; i++) {
                for (var j = 0; j < hops; j++) {
                    var coord = new Coordinates(southWestLat + latHopDist*i, southWestLng + lngHopDist*j);
                    cityCoordArray.Add(coord);
                }
            }

            return cityCoordArray;
        }

        /// <summary>
        /// fetches the events from a page and stores them into the db
        /// </summary>
        /// <param name="result"></param>
        /// <returns>a boolean indicating wether the event was new (true) or just updated (false)</returns>
        public async Task<bool> fetchAndStoreEvent(string fbId) {
            var e = await eventSearchByFbId(fbId);
            var eventDbId = Guid.Empty; //put empty Guid in Place. this is the quivalent of a NULL for an object

            //if event not already in DB OR if already in DB but not up to date
            if (e == null || e.lastUpdatedTimestamp == DateTime.MinValue ||
                (e.lastUpdatedTimestamp != DateTime.MinValue &&
                 e.lastUpdatedTimestamp - DateTime.Now.AddDays(-1) < TimeSpan.Zero)) {
                if (e != null)
                    eventDbId = e.Id; //if the Page was already in the DB, we make sure we dont loose our Guid
                //Get Event information
                var data = graphApiGet(fbId, "searchEventData");
                if (data == null && e != null) {
                    removeEventFromDb(e);
                    return false;
                }
                e = JsonConvert.DeserializeObject<Event>(data);
                e.Id = eventDbId;
                e.attending = fetchAttendingList(e.fbId);
                e = FillEmptyEventFields(e); //fill location
                //use attending list to get the genderlist
                e = await genderizeService.createGenderStat(e);
                //put the old Guid back in place or in case of a new Page the default value. we need this in our repo

                Debug.WriteLine("DB Push Event");
                var task = eventRepo.InsertAsync(e);
                task.Wait();
            }
            return (eventDbId == Guid.Empty);
        }

        public void removeEventFromDb(Event e) {
            eventRepo.DeleteAsync(e);
        }

        /// <summary>
        /// fetches all pages and stores them to db
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public async Task<Page> fetchAndStorePage(string fbId) {
            var pageInDb = pageRepo.GetByFbIdAsync(fbId).Result;
            var pageDbId = Guid.Empty; //put empty Guid in Place. this is the quivalent of a NULL for an object
            
            if (pageInDb != null) pageDbId = pageInDb.Id; //if the Page was already in the DB, we make sure we dont loose our Guid
                //Get page information
                pageInDb = JsonConvert.DeserializeObject<Page>(graphApiGet(fbId, "pageData"));
                pageInDb.Id = pageDbId;
                //put the old Guid back in place or in case of a new Page the default value. we need this in our repo
                //and push it to db
                Debug.WriteLine("DB Push Page");
                var task = pageRepo.InsertAsync(pageInDb);
                task.Wait();
            //always return page, no matter if updated or now
            return pageInDb;
        }

        /// <summary>
        /// creates an attending list
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public List<Rsvp> makeAttendingList(List<FacebookPagingResult> result) {
            return result.Select(r => new Rsvp {id = r.fbId, name = r.name, rsvp_status = r.rsvp_status}).ToList();
        }

        /// <summary>
        /// creates a grid by hopping
        /// </summary>
        /// <param name="city"></param>
        /// <param name="angle"></param>
        /// <param name="hops"></param>
        /// <returns></returns>
        public double getHopDistance(City city, String angle, int hops) {
            //First Coordinate: Southwest, Second: Northeast

            if (angle == "latitude")
                return Math.Abs((city.getPolygon()[3, 0] - city.getPolygon()[1, 0])/hops);
            return Math.Abs((city.getPolygon()[3, 1] - city.getPolygon()[1, 1])/hops);
        }

        /// <summary>
        ///     Adds GeoLocation data (latitude, longitude) to an Event. If successful the method goes on with adding the events
        ///     start and end time.
        /// </summary>
        /// <param name="e">Event modified with location data</param>
        /// <returns>modified Event if sucsessful, otherwise null</returns>
        public Event FillEmptyEventFields(Event e) {
            try {
                // Case: Facebook event belongs to a Facebook page which got a location (latitude/longitude) -> location of Event where we will put an index on.
                if (e.place != null && e.place.location != null && e.place.location.latitude != 0 &&
                    e.place.location.longitude != 0)
                    e.geoLocationCoordinates = new GeoLocation(e.place.location.latitude, e.place.location.longitude);
                /* Case: Facebook event belongs to a Facebook page where a location is not defiened 
                         but there is a field "name" which describes its location. By interacting with
                         the Google API through a http request (GraphApiGet()) which contains the locations 
                         describtion for example "Alexanderplatz, Berlin" the respond will be a JSON String.
                         From that String we extract the latitude and longitude.
                */
                else if (e.place != null && e.place.name != null) {
                    //TODO Debug 
                    var getData = e.place.name;
                    var obj = JObject.Parse(graphApiGet(getData, "GOOGLE"));
                    var arr = (Array) obj["Results"];
                    if (obj["results"] != null && arr != null && arr.Length != 0) {
                        var lat = Convert.ToDouble(obj["results"][0]["geometry"]["location"]["lat"]);
                        var lng = Convert.ToDouble(obj["results"][0]["geometry"]["location"]["lng"]);
                        e.geoLocationCoordinates = new GeoLocation(lat, lng);
                    }
                }
                else if (e.owner != null && e.owner.id != null) {
                    var page = JsonConvert.DeserializeObject<Page>(graphApiGet(e.owner.id, "pageData"));
                    if (page.location != null) {
                        e.place = new Place {
                            id = page.fbId,
                            location = page.location,
                            name = page.name
                        };
                        e.geoLocationCoordinates = new GeoLocation(e.place.location.latitude, e.place.location.longitude);
                    }
                }
                else {
                    return e;
                }
            }
            catch (NullReferenceException ex) {
                Console.Write(ex.ToString());
            }
            // Go on with filling the events start and end time fields. 
            e = fillStartEndDateTime(e);
            return e;
        }

        //Add converted fields to the event which we can later put an index on
        public Event fillStartEndDateTime(Event e) {
            if (e.start_time != null) e.startDateTime = UtilService.getDateTimeFromISOString(e.start_time);
            if (e.end_time != null) e.endDateTime = UtilService.getDateTimeFromISOString(e.end_time);
            return e;
        }
    }

}