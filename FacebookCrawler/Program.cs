using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using placeToBe.Services;
using System.IO;
using System.Threading.Tasks;

namespace FacebookCrawler {
    internal class Program {
        private MongoDbRepository<City> cityRepo = new MongoDbRepository<City>();
        private string googleMapsKey = ConfigurationManager.AppSettings.Get("GoogleMapsKey");

        private static void Main(string[] args) {
            setRedirectOutputToLogfile();
            Program p = new Program();
            while (true) {
                p.runFacebookCrawlerOverCities(p.getAllCitiesFromDb());
                Thread.Sleep(1000*60*60*4); //wait 4 hours
            }
        }

        /**
         * this method gets all the cities from the db. it also makes sure we have all the data we need, so if there is some data missing, it fetches it from the Google Maps API
         */

        protected List<City> getAllCitiesFromDb() {
            var task = cityRepo.GetAllAsync();
            var cities = task.Result;

            for (var i = 0; i < cities.Count; i++) {
                City city = cities[i];
                if (city.geometry == null || city.geometry.bounds == null || city.geometry.bounds.northeast == null) {
                    if (city.formatted_address != null) {
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

        private City fetchCityDetailFromGMapsAndStoreInDb(City city) {
            var urlEncodedAddress = HttpUtility.UrlEncode(city.formatted_address);
            var requestUrl = "https://maps.googleapis.com/maps/api/geocode/json?address=" + urlEncodedAddress + "&key=" +
                             googleMapsKey;
            var result = UtilService.performGetRequest(new Uri(requestUrl));
            var fetchedCities = JsonConvert.DeserializeObject<GMapsGeocodingResponse>(result);
            if (fetchedCities.status == "OK") {
                var fetchedCity = fetchedCities.results[0];
                fetchedCity.Id = city.Id;
                var citySaveTask = cityRepo.UpdateAsync(fetchedCity);
                citySaveTask.Wait();
                Thread.Sleep(1000*2); //otherwise google maps kicks us out :-/
                return fetchedCity;
            }
            else {
                Thread.Sleep(1000*10*60);
                return fetchCityDetailFromGMapsAndStoreInDb(city);
            }
        }

        protected void runFacebookCrawlerOverCities(List<City> cities) {

            cities.Sort((x, y) => DateTime.Compare(x.lastCheckedTime, y.lastCheckedTime));
            cities.Reverse();

            var parallelOptions = new ParallelOptions{
                MaxDegreeOfParallelism = 5
                };


            Parallel.ForEach(cities, parallelOptions,
            city => {
                FbCrawler fbCrawler = new FbCrawler();
                Debug.WriteLine(
                    "#########################################################################################");
                Debug.WriteLine(DateTime.Now);
                Debug.WriteLine("##################### FETCHING NEXT CITY: " + city.formatted_address +
                                " ################");
                Debug.WriteLine(
                    "#########################################################################################");
                city.lastCheckedTime = DateTime.Now;
                fbCrawler.performCrawlingForCity(city);
                var waitTask = cityRepo.UpdateAsync(city);
                waitTask.Wait();
            });
        }

        private static void setRedirectOutputToLogfile() {
            Trace.Listeners.Add(new TextWriterTraceListener("crawlerDebug.log"));
            Trace.AutoFlush = true;
            Trace.Indent();
        }
    }
}