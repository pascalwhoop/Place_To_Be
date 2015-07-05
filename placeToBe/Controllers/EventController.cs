using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using placeToBe.Services;

namespace placeToBe.Controllers
{
    public class EventController : ApiController
    {
        readonly SearchService search = new SearchService();
        readonly MongoDbRepository<Event> repo = new MongoDbRepository<Event>();
       
        /// <summary>
        /// Return a list of events. The method needs the id, year, month, day and hour.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <returns></returns>
        [Route("api/event/filter/{id}/{year}/{month}/{day}/{hour}")]
        public async Task<List<LightEvent>> getEventsByTimeAndCity(string id, string year, string month, string day, string hour)
        {

            DateTime time = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), 0, 0);
            return await search.HeatSearch(id, time, time.AddHours(8));
        }

        /// <summary>
        /// Returns a list of the events (from the starting point you zoom in the map) plus the description of event.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="radius"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <returns></returns>
        [Route("api/event/filter/{latitude}/{longitude}/{radius}/{year}/{month}/{day}/{hour}")]
        public async Task<List<Event>> getNearEventsByPointWithDescription(string latitude, string longitude,string radius, string year, string month, string day, string hour)
        {

            DateTime time = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), 0, 0);
            Debug.WriteLine(time);
            double latitudeDouble = double.Parse(latitude, System.Globalization.CultureInfo.InvariantCulture);
            double longitudeDouble = double.Parse(longitude, System.Globalization.CultureInfo.InvariantCulture); 
            Debug.WriteLine(latitudeDouble);
            Debug.WriteLine(longitudeDouble);
            return await search.findNearEventFromAPoint(latitudeDouble, longitudeDouble, int.Parse(radius), time, time.AddHours(8));
        }

        /// <summary>
        /// Return a certain event with a specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Event> GetEvent(Guid id)
        {
            
            Event setEvent = await search.EventSearch(id);
            return setEvent;
          
        }
    }
}
