using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using placeToBe.Model;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;

namespace placeToBe.Service {
    public class FbCrawler {
        private PageRepository pageRepo = new PageRepository();
        private EventRepository eventRepo = new EventRepository();
        private String fbAppSecret = "469300d9c3ed9fe6ff4144d025bc1148";
        private String fbAppId = "857640940981214";
        private String AppGoogleKey = "AIzaSyArx67_z9KxbrVMurzBhS2mzqDhrpz66s0";
        private String accessToken { get; set; }
        private String url;
        private QueueManager functionsQueue;
        private int maxWorkerThreads = 25;
        private int maxAsyncThreads = 2000;

        public FbCrawler() {
            accessToken = graphApiGet("", "FBAppToken");
            if (accessToken == "") {
                Debug.WriteLine("didnt receive token. retrying... ");

                for (var i = 0; i < 20; i++) {
                    Debug.Write(".");
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

        //GET Request to the Facebook GraphApi
        public String graphApiGet(String getData, String condition) //synchrone Operation
        {
            String result = "";

            if (condition == "GOOGLE")
                url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + getData + "&key=" + AppGoogleKey;
            else {
                switch (condition) {
                    case "pageData":
                        url = "https://graph.facebook.com/v2.3/" + getData + "?access_token=" + fbAppId + "|" +
                              fbAppSecret;
                        break;
                    case "searchPlace":
                        //split[0]= latitude, split[1]=longitude, split[2]=distance, split[3]=limit
                        String[] split = getData.Split(new[] {"|"}, StringSplitOptions.None);
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

            Uri uri = new Uri(url);

            Debug.WriteLine("GET: " + condition);
            return performGetRequest(uri);
        }

        private string performGetRequest(Uri URI) {
            String response = null;
            try {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(URI);
                if (stream != null) {
                    StreamReader reader = new StreamReader(stream);
                    response = reader.ReadToEnd();
                }
            }
            catch (WebException ex) {
                if (ex.Response is HttpWebResponse) {
                    switch (((HttpWebResponse) ex.Response).StatusCode) {
                        case HttpStatusCode.NotFound:
                            response = null;
                            break;

                        default:
                            throw ex;
                    }

                }
            }
            return response;

        }

        /*
         * Shuffles an array of type Coordinates
         * */

        public Coordinates[] shuffle(Coordinates[] o) {
            Random random = new Random();
            int n = o.Length;

            for (int i = 0; i < n; i++) {
                //NextDouble -> random number between 0 and 1
                int r = i + (int) (random.NextDouble()*(n - i));
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

        public async void findPagesForCities(City city) {
            String distance = "2000";
            String limit = "2000";

            //Get all Coordinates of a part of the City
            List<Coordinates> coordListCity = createCoordinatesArray(city);
            //transform list into array
            Coordinates[] coordArrayCity = coordListCity.ToArray();
            //Shuffle the Array
            Coordinates[] coordArrayCityShuffled = shuffle(coordArrayCity);

            Debug.WriteLine("WILL FETCH ALL EVENTS FOR CITY. OUR SEARCH GRID HOLDS " + coordArrayCityShuffled.Length +
                            " BLOCKS");
            Debug.WriteLine(
                "==================================================================================================");

            var i = 0;
            foreach (Coordinates coord in coordArrayCityShuffled) {
                String getData = coord.latitude + "|" + coord.longitude + "|" + distance + "|" + limit;
                getData = getData.Replace(",", ".");
                Debug.WriteLine("FETCHING BLOCK: " + i++);
                Debug.WriteLine("COORDINATES: " + coord.latitude + " --- " + coord.longitude);
                Debug.WriteLine("===========================================");
                //fetch all Places for this coordinate
                List<FacebookPagingResult> pages =
                    completePaging(
                        JsonConvert.DeserializeObject<FacebookPageResults>(graphApiGet(getData, "searchPlace")));
                //and for each of them find all events if they arent a community page
                await handlePagesList(pages);
            }
        }

        public async Task handlePagesList(List<FacebookPagingResult> pages) {
            /*Parallel.ForEach(pages, page => {
                if (!page.is_community_page) {
                    Task<Guid> t = fetchAndStorePage(page); //TODO check if page is known to host events
                    Guid guid = t.Result;
                    fetchEventsOnPage(page.fbId).Wait();
                }
            });*/
            foreach (var page in pages) {
                if (!page.is_community_page)
                {
                    Task<Guid> t = fetchAndStorePage(page); //TODO check if page is known to host events
                    Guid guid = t.Result;
                    fetchEventsOnPage(page.fbId).Wait();
                }
            }
        }

        //find events on every page in db
        public async Task fetchEventsOnPage(String pageId) {
            String response = graphApiGet(pageId, "searchEvent");
            List<FacebookPagingResult> results =
                completePaging(JsonConvert.DeserializeObject<FacebookPageResults>(response));
            foreach (FacebookPagingResult e in results) {
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
            String response = graphApiGet(eventId, "attendingList");

            List<FacebookPagingResult> r = completePaging(JsonConvert.DeserializeObject<FacebookPageResults>(response));

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
            Page pagePage = await pageRepo.GetByFbIdAsync(fbId);
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
            List<Coordinates> cityCoordArray = new List<Coordinates>();

            double latHopDist = getHopDistance(city, "latitude", hops);
            double lngHopDist = getHopDistance(city, "longitude", hops);

            double southWestLat = city.polygon[3, 0]; //SouthwestLatitude
            double southWestLng = city.polygon[3, 1]; //SouthwestLongitude

            for (int i = 0; i < hops; i++) {
                for (int j = 0; j < hops; j++) {
                    Coordinates coord = new Coordinates(southWestLat + latHopDist*i, southWestLng + lngHopDist*j);
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

        private async Task<Guid> fetchAndStoreEvent(FacebookPagingResult result) {
            Event e;
            try {
                e = await eventSearchByFbId(result.fbId);
            }
            catch (Exception ex) {
                Console.WriteLine("Exception: " + ex);
                e = null;
            }

            if (e == null) {
                
                    //Get Event information
                    var r = graphApiGet(result.fbId, "searchEventData");
                    e = JsonConvert.DeserializeObject<Event>(r);
                    //handle the Event and push it to db
                    if (e != null) {
                        //sometimes we get this idk why yet TODO
                        e.attending = fetchAttendingList(e.fbId);
                        e = FillEmptyEventFields(e);
                        Debug.WriteLine("DB Push Event");
                        return await eventRepo.InsertAsync(e);
                    }
                    else {
                        return Guid.NewGuid(); //TODO very dirty
                    }
            }
            else {
                return e.Id;    
            }
            
        }

        private async Task<Guid> fetchAndStorePage(FacebookPagingResult result) {
            Page pageInDb;
            pageInDb = await pageRepo.GetByFbIdAsync(result.fbId);

            if (pageInDb == null) {
                //Get page information
                var p = JsonConvert.DeserializeObject<Page>(graphApiGet(result.fbId, "pageData"));
                //handle the Event and push it to db
                Debug.WriteLine("DB Push Page");
                return await pageRepo.InsertAsync(p);
            }

            return pageInDb.Id;
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
                JObject _place = JObject.Parse(place);
                try {
                    Page page = JsonConvert.DeserializeObject<Page>(place);
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
                Event eventNew = JsonConvert.DeserializeObject<Event>(place);
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
                return Math.Abs((city.polygon[3, 0] - city.polygon[1, 0])/hops);
            else return Math.Abs((city.polygon[3, 1] - city.polygon[1, 1])/hops);
        }

        /// <summary>
        /// Adds GeoLocation data (latitude, longitude) to an Event. If successful the method goes on with adding the events 
        /// start and end time.
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
                    String getData = e.place.name;
                    JObject obj = JObject.Parse(graphApiGet(getData, "GOOGLE"));
                    Array arr = (Array) obj["Results"];
                    if (obj["results"] != null && arr != null && arr.Length != 0) {
                        double lat = Convert.ToDouble(obj["results"][0]["geometry"]["location"]["lat"]);
                        double lng = Convert.ToDouble(obj["results"][0]["geometry"]["location"]["lng"]);
                        e.geoLocationCoordinates = new GeoLocation(lat, lng);
                    }
                    // If there is no possibility to get the latitude and longitude of an event it won't be safed in the MongoDB
                }
                else return null;
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
                if (actionAvailable) ThreadPool.QueueUserWorkItem(new WaitCallback((_) => { action.Invoke(); }));
            } while (true);
        }

        public void AddToDbQueue(Action action) {
            dbOperationsQueue.TryAdd(action);
        }

        #endregion
    }
}