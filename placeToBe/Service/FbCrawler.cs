﻿using Newtonsoft.Json.Linq;
using placeToBe.Model;
using placeToBe.Model.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using placeToBe.Model.Repositories;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace placeToBe.Service
{
    public class FbCrawler
    {
        PageRepository pageRepo = new PageRepository();
        EventRepository eventRepo = new EventRepository();
        String fbAppSecret = "469300d9c3ed9fe6ff4144d025bc1148";
        String fbAppId = "857640940981214";
        String AppGoogleKey = "AIzaSyArx67_z9KxbrVMurzBhS2mzqDhrpz66s0";
        String accessToken { get; set; }
        String url;
        QueueManager functionsQueue;
        int maxWorkerThreads = 25;
        int maxAsyncThreads = 2000;

        public FbCrawler()
        {
            accessToken = GraphApiGet("", "FBAppToken");


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
        public String GraphApiGet(String getData, String condition) //synchrone Operation
        {
            String result = null;
            HttpWebRequest request;

            if (condition == "GOOGLE")
            {
                url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + getData + "&key=" + AppGoogleKey;
            }
            else if (condition == "pageData")
            {
                url = "https://graph.facebook.com/v2.3/" + getData + "?access_token=" + fbAppId + "|" + fbAppSecret;
            }
            else if (condition == "searchPlace")
            {
                //split[0]= latitude, split[1]=longitude, split[2]=distance, split[3]=limit
                String[] split = getData.Split(new string[] { "|" }, StringSplitOptions.None);
                url = "https://graph.facebook.com/v2.3/search?q=&type=place&center=" + split[0] + "," + split[1] + "&distance=" + split[2] + "&limit=" + split[3] + "&fields=id,is_community_page&access_token=" + fbAppId + "|" + fbAppSecret;
            }
            else if (condition == "nextPage")
            {
                url = getData;
            }
            else if (condition == "searchEvent")
            {
                url = "https://graph.facebook.com/v2.3/" + getData + "/events?limit=2000&fields=id&" + accessToken;
            }
            else if (condition == "searchEventData")
            {
                url = "https://graph.facebook.com/v2.3/" + getData + "/?fields=id,name,description,owner,attending_count,declined_count,maybe_count,start_time,end_time,place,cover&" + accessToken;
            }
            else if (condition == "attendingList")
            {
                url = "https://graph.facebook.com/v2.3/" + getData + "/attending?limit=2000&" + accessToken;
            }
            else if (condition == "FBAppToken")
            {
                url = "https://graph.facebook.com/oauth/access_token?client_id=" + fbAppId + "&client_secret=" +
                      fbAppSecret + "&grant_type=client_credentials";
            }

            Uri uri = new Uri(url);

            request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.AllowAutoRedirect = true;

            Debug.WriteLine("\n### GETTING: " + condition + "  " + getData);

            //var waitHandle = new ManualResetEvent(false);
            
            //ThreadPool.QueueUserWorkItem(new WaitCallback((_)=>{
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
                            result = readStream.ReadToEnd();
                            //waitHandle.Set();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                result = null;
            }
            finally
            {
                request.Abort();
            }

            //}));
            //waitHandle.WaitOne();
            return result;
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
            List<Coordinates> coordListCity = createCoordinatesArray(city);
            //transform list into array
            Coordinates[] coordArrayCity = coordListCity.ToArray();
            //Shuffle the Array
            Coordinates[] coordArrayCityShuffled = Shuffle(coordArrayCity);

            foreach (Coordinates coord in coordArrayCityShuffled)
            {
                String getData = coord.latitude + "|" + coord.longitude + "|" + distance + "|" + limit;
                getData = getData.Replace(",", ".");
                String placesListJson = GraphApiGet(getData, "searchPlace");
                Event eventNew = new Event();
                handleListPagingResponse(placesListJson, "nextPage", eventNew);
            }
        }


        //find events on every page in db
        public void fetchEventsOnPage(String pageId)
        {
            Event eventNew = new Event();


            String response = GraphApiGet(pageId, "searchEvent");
            handleListPagingResponse(response, "searchEvent", eventNew);
        }

        //GEt attending List of an event
        public void fetchAttendingList(Event eventNew)
        {

            String response = GraphApiGet(eventNew.fbId, "attendingList");
            handleListPagingResponse(response, "attendingList", eventNew);
        }

        /**
        * this function handles the response from the facebook API query of form
        * /search?q=<query>&type=place&center=<lat,lng>&distance<distance>. We want to make sure we get all the places and
        * facebook uses paging so we got to go ahead and follow through the paging process until there is no more paging
        * @param response
        */
        public void handleListPagingResponse(String response, String condition, Event eventNew)
        {
            List<String> idList = new List<String>();
            List<String> newIdsList = new List<String>();

            //GEt from GraphApi
            newIdsList = HandlePlacesResponse(response, condition);
            //Get NExt Page
            int listSize = newIdsList.Count;
            if (listSize > 0)
            {
                String nextPage = newIdsList[listSize - 1];
                //Search for more Pages until the end
                while (nextPage.Substring(0, 5) == "https")
                {
                    //Delete Page from id List
                    newIdsList.RemoveAt(listSize - 1);
                    //Merge lists
                    idList.AddRange(newIdsList);
                    //clear addPlaceIdList
                    newIdsList.Clear();
                    //Get from Graph APi
                    String nextResponse = GraphApiGet(nextPage, "nextPage");
                    newIdsList = HandlePlacesResponse(nextResponse, condition);
                    //Get NExt Page
                    listSize = newIdsList.Count;
                    if (listSize > 0)
                    {
                        nextPage = newIdsList[listSize - 1];
                    }
                    else
                    {
                        nextPage = "UNDEFINED";
                        condition = "searchPlace";
                    }
                }
            }
            
            //Merge lists last time
            idList.AddRange(newIdsList);
            //Paging complete now the next step to get more information with these id's
            //ThreadPool.QueueUserWorkItem(new WaitCallback(
            //    (_) =>
            //    {
            fetchDataForFbIdArray(idList.ToArray(), condition, eventNew);
            //}));
        }
        public List<String> HandlePlacesResponse(String response, String getData)
        {
            //new List for FB API returned values
            var idList = new List<String>();
            if (response == null) return idList;
            if (getData == "attendingList") handleAttendingListResponse(response, idList);
            else handlePageOrEventListResponse(response, idList);
            return idList;

        }

        private static void handlePageOrEventListResponse(string response, List<string> placeIdList) {
//Convert Json to c# Object facebookPageResults
            FacebookPageResults facebookPageResults;
            facebookPageResults = JsonConvert.DeserializeObject<FacebookPageResults>(response);
            //get the data part of the FacebookPageresults which contain the id's
            FacebookResults[] data = facebookPageResults.data;
            //add the id's to list
            if (data != null) foreach (FacebookResults facebookResults in data) placeIdList.Add(facebookResults.id);

            if (facebookPageResults.paging != null && facebookPageResults.paging.next != null) {
                //add the nextPage response at the end of the list, for the next request
                String next = facebookPageResults.paging.next;
                placeIdList.Add(next);
            }
        }

        private static void handleAttendingListResponse(string response, List<string> placeIdList) {
//Convert Json to c# Object facebookPageResults
            FacebookPageResultsAttending facebookPageResults;
            facebookPageResults = JsonConvert.DeserializeObject<FacebookPageResultsAttending>(response);
            //get the data part of the FacebookPageresults which contain the id's
            ResultAttending[] data = facebookPageResults.data;
            //add the id's to list
            foreach (ResultAttending facebookResults in data)
                placeIdList.Add(facebookResults.id + "," + facebookResults.name + "," + facebookResults.rsvp_status);

            if (facebookPageResults.paging != null && facebookPageResults.paging.next != null) {
                //add the nextPage response at the end of the list, for the next request
                String next = facebookPageResults.paging.next;
                placeIdList.Add(next);
            }
        }

        public async Task<Page> PageSearchDb(String fbId)
        {
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

        public async Task<Event> EventSearchByFbId(String fbId)
        {
            return await eventRepo.GetByFbIdAsync(fbId);
        }
        /**
        * returns a 50x50 array with coordinates of the form {lat: Number, lng: Number}
        * @param city
        */
        public List<Coordinates> createCoordinatesArray(City city)
        {
            int hops = 100;
            List<Coordinates> cityCoordArray = new List<Coordinates>();

            double latHopDist = GetHopDistance(city, "latitude", hops);
            double lngHopDist = GetHopDistance(city, "longitude", hops);

            double southWestLat = city.polygon[3, 0];//SouthwestLatitude
            double southWestLng = city.polygon[3, 1];//SouthwestLongitude

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
        public async void fetchDataForFbIdArray(String[] ids, String condition, Event eventNewZ)
        {

            if (condition == "attendingList" && eventNewZ.attendingCount > 15)
            {
                HandleAttendingList(ids, eventNewZ);
                return;
            }

            foreach (String fbId in ids) {
                if (condition == "searchPlace") {
                    await fetchDataForFbPage(condition, fbId);
                }

                if (condition == "searchEvent") {
                    await fetchDataForFbEvent(condition, fbId);
                }
            }
            
        }

        private async Task fetchDataForFbEvent(string condition, string fbId) {
            Event eventNew;
            try {
                eventNew = await EventSearchByFbId(fbId);
            }
            catch (Exception e) {
                Console.WriteLine("Exception: " + e);
                eventNew = null;
            }

            if (eventNew == null) {
                //Get Event information
                String eventData = GraphApiGet(fbId, "searchEventData");
                //handle the Event and push it to db
                HandlePlace(eventData, condition, fbId);
            }
        }

        private async Task fetchDataForFbPage(string condition, string fbId) {
            Page page;
            try {
                page = await PageSearchDb(fbId);
            }
            catch (Exception e) {
                Console.WriteLine("Exception: " + e);
                page = null;
            }

            if (page == null) {
                //Get page information
                String pageData = GraphApiGet(fbId, "pageData");
                //handle the page and push it to db
                HandlePlace(pageData, condition, "");
            }
            else {
                String pageData = JsonConvert.SerializeObject(page);
                fetchEventsOnPage(page.fbId);
                HandlePlace(pageData, condition, "");
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
            functionsQueue.AddToDbQueue(() => PushEventToDb(eventNew, list));
            //PushEventToDb(eventNew, list);
        }

        /**
        * handles a single FB place and saves it in the DB
        * @param place
        * @param callback
        */
        public void HandlePlace(String place, String condition, String id)
        {
            if (place == null) return;
            //Place
            if (condition == "searchPlace")
            {
                JObject _place = JObject.Parse(place);
                try
                {
                    var isCommunityPage = (bool)_place["is_community_page"];
                    JToken token = _place["GeoLocation"];
                    if (isCommunityPage == false)
                    {
                        //we only save non-community-pages since only they will actually create events
                        //Convert json to Object
                        Page page = JsonConvert.DeserializeObject<Page>(place);
                        //a place that is not community owned is == to a page in the facebook world
                        //insert in db
                        //pushPageToDb(page);
                        functionsQueue.AddToDbQueue(() => pushPageToDb(page));
                        fetchEventsOnPage(page.fbId);
                    }
                    else if (isCommunityPage == true)
                    {
                        //CommunityPage save?
                    }
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine("Exception: " + ex);
                }
            }
            //Event
            else if (condition == "searchEvent")
            {
                
                Event eventNew = JsonConvert.DeserializeObject<Event>(place);
                fetchAttendingList(eventNew);
            }
        }

        //Insert page to Db
        public async void pushPageToDb(Page newPage)
        {
            try
            {
                Task<System.Guid> pageTask = null;
                var waitHandler = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(new WaitCallback((_) =>
                {

                    Debug.WriteLine("\n**** PAGE: " + newPage.fbId);
                    pageTask = pageRepo.InsertAsync(newPage);
                    waitHandler.Set();
                }));
                waitHandler.WaitOne();
                await pageTask;

            }
            catch (MongoWaitQueueFullException ex)
            {
                //Thread.Sleep(15000);
                functionsQueue.AddToDbQueue(() => pushPageToDb(newPage));

                //pushPageToDb(newPage);
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Code == 11000) {
                    //this just means the object is already in the DB most of the time.                    
                }
            }
            catch (MongoConnectionException ex)
            {
                pageRepo = new PageRepository();
                functionsQueue.AddToDbQueue(() => pushPageToDb(newPage));
                //pushPageToDb(newPage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToJson());
            }
        }

        //Inster event to db
        public async Task PushEventToDb(Event newEvent, List<Rsvp> list)
        {
            newEvent.attending = list;
            newEvent = FillEmptyEventFields(newEvent);
            if (newEvent != null && newEvent.attendingCount > 15) //now we get old stuff too because we need the content but TODO later filter && newEvent.startDateTime > new DateTime()
            { //if event exists and more than 15 people joined and is in future persist
                try
                {
                    var waitHandler = new ManualResetEvent(false);
                    Task<System.Guid> pushResult = null;                    

                    ThreadPool.QueueUserWorkItem(new WaitCallback((_) =>
                    {
                        return eventRepo.InsertAsync(newEvent);
                        waitHandler.Set();
                        Debug.WriteLine("\n**** EVENT: " + newEvent.fbId);
                    }));
                    
                    waitHandler.WaitOne();
                    
                    
                }
                catch (MongoWriteException e)
                {
                    if (e.WriteError.Code == 11000)
                    {
                        //this just means the object is already in the DB most of the time.                    
                    }
                }
                catch (MongoWaitQueueFullException ex)
                {
                    //Thread.Sleep(15000);
                    functionsQueue.AddToDbQueue(() => PushEventToDb(newEvent, list));
                    //PushEventToDb(newEvent, list);
                }
                catch (MongoConnectionException ex)
                {
                    eventRepo = new EventRepository();
                    functionsQueue.AddToDbQueue(() => PushEventToDb(newEvent, list));
                    //PushEventToDb(newEvent, list);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToJson());
                }
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
        /// Adds GeoLocation data (latitude, longitude) to an Event. If successful the method goes on with adding the events 
        /// start and end time.
        /// </summary>
        /// <param name="e">Event modified with location data</param>
        /// <returns>modified Event if sucsessful, otherwise null</returns>
        public Event FillEmptyEventFields(Event e)
        {
            try
            {
                // Case: Facebook event belongs to a Facebook page which got a location (latitude/longitude) -> location of Event where we will put an index on.
                if (e.place != null && e.place.location != null && e.place.location.latitude != 0 && e.place.location.longitude != 0) {
                    e.geoLocationCoordinates = new GeoLocation(e.place.location.latitude, e.place.location.longitude);
                }
                /* Case: Facebook event belongs to a Facebook page where a location is not defiened 
                         but there is a field "name" which describes its location. By interacting with
                         the Google API through a http request (GraphApiGet()) which contains the locations 
                         describtion for example "Alexanderplatz, Berlin" the respond will be a JSON String.
                         From that String we extract the latitude and longitude.
                */      
                else if (e.place != null && e.place.name != null)
                {
                    String getData = e.place.name;
                    JObject obj = JObject.Parse(GraphApiGet(getData, "GOOGLE"));
                    Array arr = (Array)obj["Results"];
                    if (obj["results"] != null && arr != null && arr.Length != 0)
                    {
                        double lat = Convert.ToDouble(obj["results"][0]["geometry"]["location"]["lat"]);
                        double lng = Convert.ToDouble(obj["results"][0]["geometry"]["location"]["lng"]);
                        e.geoLocationCoordinates = new GeoLocation(lat, lng);
                    }
                // If there is no possibility to get the latitude and longitude of an event it won't be safed in the MongoDB
                }else 
                {
                    return null;
                }
            }
            catch (NullReferenceException ex)
            {
                Console.Write(ex.ToString());
            }
            // Go on with filling the events start and end time fields. 
            e = fillStartEndDateTime(e);
            return e;
        }
        //Add converted fields to the event which we can later put an index on
        public Event fillStartEndDateTime(Event e) {
            if (e.start_time != null) {
                e.startDateTime = UtilService.getDateTimeFromISOString(e.start_time);
            }
            if (e.end_time != null)
            {
                e.endDateTime = UtilService.getDateTimeFromISOString(e.end_time);
            }
            return e;
        }

    }


    #region QueueManager

    public class QueueManager
    {
        BlockingCollection<Action> dbOperationsQueue;
        Thread workerDb;

        public QueueManager()
        {
            StartDbOperations();

        }

        //public void handlePlacesIdQueue()
        //{

        //}
        public void StartDbOperations()
        {
            dbOperationsQueue = new BlockingCollection<Action>();
            workerDb = new Thread(DoWorkDbQueue);
            workerDb.IsBackground = true;
            workerDb.Start();
        }

        private void DoWorkDbQueue()
        {
            bool actionAvailable;
            do
            {


                Action action;
                actionAvailable = dbOperationsQueue.TryTake(out action, Timeout.Infinite);
                if (actionAvailable)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback((_) =>
                    {
                        action.Invoke();
                    }));
                }
            } while (true);
        }

        public void AddToDbQueue(Action action)
        {
            dbOperationsQueue.TryAdd(action);
        }


    #endregion
    }
}