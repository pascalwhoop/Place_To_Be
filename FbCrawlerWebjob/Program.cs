using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using placeToBe.Services;
using placeToBe.Model;
using ThreadState = System.Threading.ThreadState;

namespace FbCrawlerWebjob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        private MongoDbRepository<City> cityRepo = new MongoDbRepository<City>();
        private string MAPS_KEY = ConfigurationManager.AppSettings.Get("GoogleMapsKey");
        private readonly PageRepository pageRepo = new PageRepository();
        private readonly EventRepository eventRepo   = new EventRepository();
        
        private readonly int PLACE_SEARCH_INTERVAL_DAYS = 
            int.Parse(ConfigurationManager.AppSettings.Get("placeSearchIntervalInDays"));
        private readonly int PAGE_CRAWL_INTERVAL_DAYS =
            int.Parse(ConfigurationManager.AppSettings.Get("pageCrawlIntervalInDays"));
        private readonly int EVENT_UPDATE_INTERVAL_DAYS =
            int.Parse(ConfigurationManager.AppSettings.Get("eventUpdateIntervalInDays"));

        private BackgroundWorker gridSearchWorker;
        private BackgroundWorker pageCrawlWorker;
        private BackgroundWorker eventCrawlWorker;


        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main() {

            //Thread.Sleep(1000*40); // to be able to debug and fetch the first few things. 
            setRedirectOutputToLogfile();

            // The following code ensures that the WebJob will be running continuously
            Program p = new Program();

            while (true) {
                Thread.Sleep(1000);
            }
        }

        public Program() {
            //start a new Background Task crawling the Entire DB's cities. This one finds all public places and adds them to the DB. the crawlers below will take care of the rest
            gridSearchWorker = new BackgroundWorker();
            gridSearchWorker.DoWork += runGridSearch;
            var gridSearchTimer = new System.Timers.Timer(1000*60*60*24*14);
            gridSearchTimer.Elapsed += gridSearchtimerElapsed;
            gridSearchTimer.Start();
            gridSearchWorker.RunWorkerAsync();

            //start event crawl worker. this one just fetches all the events again that are already in the database
            eventCrawlWorker = new BackgroundWorker();
            eventCrawlWorker.DoWork += runEventCrawl;
            var eventSearchTimer = new System.Timers.Timer(1000*60*60*24);
            eventSearchTimer.Elapsed += eventSearchTimerElapsed;
            eventSearchTimer.Start();
            eventCrawlWorker.RunWorkerAsync();

            //page crawl worker. this one looks at all the pages in the Db and fetches all events that arent yet in the DB
            pageCrawlWorker = new BackgroundWorker();
            pageCrawlWorker.DoWork += runPageCrawl;
            var pageCrawlTimer = new System.Timers.Timer(1000*60*60*24*2);
            pageCrawlTimer.Elapsed += pageCrawlTimerElapsed;
            pageCrawlTimer.Start();
            pageCrawlWorker.RunWorkerAsync();
        }


        public void gridSearchtimerElapsed(object sender, ElapsedEventArgs e)
        {
            if(!gridSearchWorker.IsBusy) gridSearchWorker.RunWorkerAsync();
        }

        public void eventSearchTimerElapsed(object sender, ElapsedEventArgs e) {
            if(!eventCrawlWorker.IsBusy) eventCrawlWorker.RunWorkerAsync();
        }

        public void pageCrawlTimerElapsed(object sender, ElapsedEventArgs e) {
            if (!pageCrawlWorker.IsBusy) pageCrawlWorker.RunWorkerAsync();
        }


        /// <summary>
        /// Fetches all cities from db and if city has empty values, this fields of the city get filled
        /// </summary>
        /// <returns>all cities from db</returns>


        protected async void runEventCrawl(object o , DoWorkEventArgs args) {
            FbEventSpecificCrawler fbEventCrawler = new FbEventSpecificCrawler();
            await fbEventCrawler.updateAllEventsInDb();
        }

        protected async void runPageCrawl(object o, DoWorkEventArgs args) {
            FbPageSpecificCrawler fbPageCrawler = new FbPageSpecificCrawler();
            await fbPageCrawler.findNewEventsOnPagesInDb();
        }

        /// <summary>
        /// start the facebookcrawler with a list of cities
        /// </summary>
        /// <param name="cities"></param>
        protected void runGridSearch(object o, DoWorkEventArgs args) {
            var cities = getAllCitiesFromDb();
            //check when city got last time fetched
            cities.Sort((x, y) => DateTime.Compare(x.lastCheckedTime, y.lastCheckedTime));
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 3 };

            //do parallel crawling with MaxDegreeOfParallelism(Value) threads
            Parallel.ForEach(cities, parallelOptions,
            city =>
            {
                FbCrawler fbCrawler = new FbCrawler();
                writeCityLog(city);
                city.lastCheckedTime = DateTime.Now;
                cityRepo.UpdateAsync(city);
                fbCrawler.performCrawlingForCity(city);
                var waitTask = cityRepo.UpdateAsync(city);
                waitTask.Wait();
            });
        }

        private void writeCityLog(City city) {
            Debug.WriteLine(
                    "#########################################################################################");
            Debug.WriteLine(DateTime.Now);
            Debug.WriteLine("##################### FETCHING NEXT CITY: " + city.formatted_address +
                            " ################");
            Debug.WriteLine(
                "#########################################################################################");
        }
        /// <summary>
        /// save to log file
        /// </summary>
        private static void setRedirectOutputToLogfile()
        {
            Trace.Listeners.Add(new TextWriterTraceListener("crawlerDebug.log"));
            Trace.AutoFlush = true;
            Trace.Indent();
        }

        protected List<City> getAllCitiesFromDb()
        {
            var task = cityRepo.GetAllAsync();
            var cities = task.Result;

            for (var i = 0; i < cities.Count; i++)
            {
                City city = cities[i];
                if (city.geometry == null || city.geometry.bounds == null || city.geometry.bounds.northeast == null)
                {
                    if (city.formatted_address != null)
                    {
                        var fetchedCity = fetchCityDetailFromGMapsAndStoreInDb(city);
                        cities[i] = fetchedCity;
                    }
                }
            }
            return cities as List<City>;
        }

        /**
         * fetches a cities details from the google maps api and stores the updated city in the DB. 
         */

        private City fetchCityDetailFromGMapsAndStoreInDb(City city)
        {
            var urlEncodedAddress = HttpUtility.UrlEncode(city.formatted_address);
            var requestUrl = "https://maps.googleapis.com/maps/api/geocode/json?address=" + urlEncodedAddress + "&key=" +
                             MAPS_KEY;
            var result = UtilService.performGetRequest(new Uri(requestUrl));
            var fetchedCities = JsonConvert.DeserializeObject<GMapsGeocodingResponse>(result);
            if (fetchedCities.status == "OK")
            {
                var fetchedCity = fetchedCities.results[0];
                fetchedCity.Id = city.Id;
                var citySaveTask = cityRepo.UpdateAsync(fetchedCity);
                citySaveTask.Wait();
                Thread.Sleep(1000 * 2); //otherwise google maps kicks us out :-/
                return fetchedCity;
            }
            else
            {
                Thread.Sleep(1000 * 10 * 60);
                return fetchCityDetailFromGMapsAndStoreInDb(city);
            }
        }
    }
}
