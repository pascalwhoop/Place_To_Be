using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private readonly String fbAppSecret = "469300d9c3ed9fe6ff4144d025bc1148";
        private readonly String fbAppId = "857640940981214";
        private readonly String AppGoogleKey = "AIzaSyArx67_z9KxbrVMurzBhS2mzqDhrpz66s0";
        private String accessToken { get; set; }
        private String url;
        private readonly QueueManager functionsQueue;
        private readonly int maxWorkerThreads = 25;
        private readonly int maxAsyncThreads = 2000;

        /**
         * constructor. get the fb acces token to use the fb api and store it. in case it doesnt work we retry 20 times
         */

        public FbCrawler() {
            accessToken = graphApiGet("", "FBAppToken");
            if (accessToken == "") {
                Debug.WriteLine("didnt receive token. retrying... ");

                for (var i = 0; i < 20; i++) {
                    Debug.Write(".");
                    Thread.Sleep(1000);
                    accessToken = graphApiGet("", "FBAppToken");
                }
            }

            //Limit Threads
            ThreadPool.SetMaxThreads(maxWorkerThreads, maxAsyncThreads);
            functionsQueue = new QueueManager();
        }

        ////Get the accesToken for the given AppSecret and AppId
        //public void AuthenticateWithFb(String fbAppId, String fbAppSecret)
        //{
        //    String _response=GraphApiGet("oauth/access_token?client_id="+fbAppId+"&client_secret="+fbAppSecret+"&grant_type=client_credentials");
        //    //String [] split = _response.Split(new char[]{'|'});
        //    accessToken = _response;
        //}

        //GET Request to the WEB APIs we use. the parameter condition 
        public String graphApiGet(String getData, String requestToPerform) //synchrone Operation
        {
            var result = "";

            if (requestToPerform == "GOOGLE")
                url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + getData + "&key=" + AppGoogleKey;
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
                              "/events?limit=2000&fields=id,attending_count&" + accessToken;
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

        /**
         * Perform an actual GET Request to the specified URL. just a simple one, a synchronous operation that returns the response as a string 
         */

        /*
         * Shuffles an array
         * */

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

        /**
        * queriing FB API in a grid like fashion to find all pages within a city. this is very intense on the API which is why
        * we shouldn't do this often TODO right now we query 50x50 grid (2500 * (query*paging/query)) queries. So if we have
        * to do paging 3x per query its 7500 calls to the API.. yeah might be obvious what we intend
        * @param city
        */

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

            var i = 0;
            foreach (var coord in coordArrayCityShuffled) {
                var getData = coord.latitude + "|" + coord.longitude + "|" + distance + "|" + limit;
                getData = getData.Replace(",", ".");
                Debug.WriteLine("FETCHING BLOCK: " + i++);
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

        /**
         * we now have a list of {id: ..., is_community_page: true/false} .. we process this list. all community pages dont get called, for the others we fetch the details and the events
         */

        public async Task handlePagesList(List<FacebookPagingResult> pages) {
            /*Parallel.ForEach(pages, page => {
                if (!page.is_community_page) {
                    Task<Guid> t = fetchAndStorePage(page); //TODO check if page is known to host events
                    Guid guid = t.Result;
                    fetchEventsOnPage(page.fbId).Wait();
                }
            });*/
            foreach (var page in pages) {
                if (!page.is_community_page) {
                    try {
                        var t = fetchAndStorePage(page); //TODO check if page is known to host events
                        var p = t.Result;
                        if (determineIfEventsShouldBeFetched(p))
                            fetchEventsOnPage(p.fbId).Wait();
                        //TODO check if events have been fetched within the last 24 h
                    }
                    catch (Exception exception) {
                        Debug.WriteLine(exception); // TODO VERY DIRTY FIX ASAP
                    }
                }
            }
        }

        //check if we should check the pages Events (only if page update is over 1 day ago or under 5 seconds ago)
        private static bool determineIfEventsShouldBeFetched(Page p) {
            bool shouldFetchEvents;
            var pLastChecked = p.lastUpdatedTimestamp;
            var now = DateTime.Now;
            var timeDiff = now - pLastChecked;
            if (timeDiff < TimeSpan.FromSeconds(5) || timeDiff > TimeSpan.FromHours(24)) shouldFetchEvents = true;
            else shouldFetchEvents = false;
            return shouldFetchEvents;
        }

        //find events on every page in db
        public async Task fetchEventsOnPage(String pageId) {
            var response = graphApiGet(pageId, "searchEvent");
            var results =
                completePaging(JsonConvert.DeserializeObject<FacebookPageResults>(response));
            foreach (var e in results) {
                if (e.attending_count > 5) {
                    try {
                        await fetchAndStoreEvent(e);
                    }
                    catch (Exception ex) {
                        Debug.WriteLine(ex.StackTrace);
                    }
                }
            }
        }

        //GEt attending List of an event
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

            //Paging complete now the next step to get more information with these id's
            //ThreadPool.QueueUserWorkItem(new WaitCallback(
            //    (_) =>
            //    {
            return pagingResults;

            //}));
        }

        public async Task<Page> pageSearchDb(String fbId) {
            //Task<Page> page = null;
            //var waitHandler = new ManualResetEvent(false);
            //ThreadPool.QueueUserWorkItem(new WaitCallback((_) =>
            //{
            var pagePage = await pageRepo.GetByFbIdAsync(fbId);
            //    waitHandler.Set();
            //}));
            //waitHandler.WaitOne();
            //Page pagePage = await page;
            //if (pagePage == null || page==null)
            //{
            //    System.Diagnostics.Debug.WriteLine("nicht geklappt");

            //}
            return pagePage;
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

        /**
         * handles an array of placeIDs and gets the full information for each of them from the FB API
         * @param arr
         */
        /*public async void fetchDataForFbIdArray(FacebookPagingResult[] results, String condition, Event eventNewZ)
        {

            if (condition == "attendingList" && eventNewZ.attendingCount > 15)
            {
                eventNewZ.attending = makeAttendingList(results, eventNewZ);
                functionsQueue.AddToDbQueue(() => eventRepo.InsertAsync(eventNewZ));
                return;
            }

            foreach (FacebookPagingResult result in results) {
                if (condition == "searchPlace" && result.is_community_page != true) {
                    
                    await fetchAndStorePage(condition, result);
                }

                if (condition == "searchEvent" && result.attending_count > 15) {
                    await fetchAndStoreEvent(condition, result);
                }
            }
            
        }*/

        private async Task<Event> fetchAndStoreEvent(FacebookPagingResult result) {
            var e = await eventSearchByFbId(result.fbId);
            var eventDbId = Guid.Empty; //put empty Guid in Place. this is the quivalent of a NULL for an object

            //if event not already in DB OR if already in DB but not up to date
            if (e == null || e.lastUpdatedTimestamp == DateTime.MinValue ||
                (e.lastUpdatedTimestamp != DateTime.MinValue &&
                 e.lastUpdatedTimestamp - DateTime.Now.AddDays(-1) < TimeSpan.Zero)) {
                if (e != null)
                    eventDbId = e.Id; //if the Page was already in the DB, we make sure we dont loose our Guid
                //Get Event information
                e = JsonConvert.DeserializeObject<Event>(graphApiGet(result.fbId, "searchEventData"));
                e.Id = eventDbId;
                e = FillEmptyEventFields(e); //fill location
                //put the old Guid back in place or in case of a new Page the default value. we need this in our repo

                Debug.WriteLine("DB Push Event");
                var task = eventRepo.InsertAsync(e);
                task.Wait();
            }
            return e;
        }

        private async Task<Page> fetchAndStorePage(FacebookPagingResult result) {
            var pageInDb = pageRepo.GetByFbIdAsync(result.fbId).Result;
            var pageDbId = Guid.Empty; //put empty Guid in Place. this is the quivalent of a NULL for an object

            //(if page is not yet in DB) OR (if it has been updated in the last 7 days) we update and push to DB
            if (pageInDb == null || pageInDb.lastUpdatedTimestamp == DateTime.MinValue ||
                (pageInDb.lastUpdatedTimestamp != DateTime.MinValue &&
                 pageInDb.lastUpdatedTimestamp - DateTime.Now.AddDays(-7) < TimeSpan.Zero)) {
                if (pageInDb != null)
                    pageDbId = pageInDb.Id; //if the Page was already in the DB, we make sure we dont loose our Guid
                //Get page information
                pageInDb = JsonConvert.DeserializeObject<Page>(graphApiGet(result.fbId, "pageData"));
                pageInDb.Id = pageDbId;
                //put the old Guid back in place or in case of a new Page the default value. we need this in our repo
                //and push it to db
                Debug.WriteLine("DB Push Page");
                var task = pageRepo.InsertAsync(pageInDb);
                task.Wait();
            }
            //always return page, no matter if updated or now
            return pageInDb;
        }

        public List<Rsvp> makeAttendingList(List<FacebookPagingResult> result) {
            return result.Select(r => new Rsvp {id = r.fbId, name = r.name, rsvp_status = r.rsvp_status}).ToList();
        }

        /**
        * handles a single FB place and saves it in the DB
        * @param place
        * @param callback
        */

        public void handlePlace(String place, String condition, String id) {
            if (place == null) return;
            //Place
            if (condition == "searchPlace") {
                var _place = JObject.Parse(place);
                try {
                    var page = JsonConvert.DeserializeObject<Page>(place);
                    //a place that is not community owned is == to a page in the facebook world
                    //insert in db
                    //pushPageToDb(page);
                    functionsQueue.AddToDbQueue(() => pushPageToDb(page));
                    fetchEventsOnPage(page.fbId);
                }
                catch (ArgumentNullException ex) {
                    Console.WriteLine("Exception: " + ex);
                }
            }
            //Event
            else if (condition == "searchEvent") {
                var eventNew = JsonConvert.DeserializeObject<Event>(place);
                fetchAttendingList(eventNew.fbId);
            }
        }

        //Insert page to Db
        public async void pushPageToDb(Page newPage) {
            try {
                Task<Guid> pageTask = null;
                var waitHandler = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(_ => {
                    Debug.WriteLine("\n**** PAGE: " + newPage.fbId);
                    pageTask = pageRepo.InsertAsync(newPage);
                    waitHandler.Set();
                });
                waitHandler.WaitOne();
                await pageTask;
            }
            catch (MongoWaitQueueFullException ex) {
                //Thread.Sleep(15000);
                functionsQueue.AddToDbQueue(() => pushPageToDb(newPage));

                //pushPageToDb(newPage);
            }
            catch (MongoWriteException ex) {
                if (ex.WriteError.Code == 11000) {
                    //this just means the object is already in the DB most of the time.                    
                }
            }
            catch (MongoConnectionException ex) {
                pageRepo = new PageRepository();
                functionsQueue.AddToDbQueue(() => pushPageToDb(newPage));
                //pushPageToDb(newPage);
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.ToJson());
            }
        }

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

    #region QueueManager

    public class QueueManager {
        private BlockingCollection<Action> dbOperationsQueue;
        private Thread workerDb;

        public QueueManager() {
            startDbOperations();
        }

        //public void handlePlacesIdQueue()
        //{

        //}
        public void startDbOperations() {
            dbOperationsQueue = new BlockingCollection<Action>();
            workerDb = new Thread(DoWorkDbQueue);
            workerDb.IsBackground = true;
            workerDb.Start();
        }

        private void DoWorkDbQueue() {
            bool actionAvailable;
            do {
                Action action;
                actionAvailable = dbOperationsQueue.TryTake(out action, Timeout.Infinite);
                if (actionAvailable) ThreadPool.QueueUserWorkItem(_ => { action.Invoke(); });
            } while (true);
        }

        public void AddToDbQueue(Action action) {
            dbOperationsQueue.TryAdd(action);
        }

        #endregion
    }
}