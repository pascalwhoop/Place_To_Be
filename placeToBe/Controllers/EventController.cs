using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using placeToBe.Model.Entities;
using placeToBe.Services;

namespace placeToBe.Controllers
{
    public class EventController : ApiController
    {
        readonly SearchService search = new SearchService();

        /// <summary>
        /// Return a list of events. The method needs the id, year, month, day and hour.
        /// </summary>
        /// <param name="id">City id</param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <returns></returns>
        [Route("api/event/filter/{id}/{year}/{month}/{day}/{hour}")]
        [PlaceToBeAuthenticationFilter]
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
        [PlaceToBeAuthenticationFilter]
        public async Task<List<Event>> getNearEventsByPointWithDescription(string latitude, string longitude,string radius, string year, string month, string day, string hour)
        {
            HttpContext httpContext = HttpContext.Current;
            Debug.WriteLine(httpContext);

            DateTime time = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), 0, 0);
            Debug.WriteLine(time);
            double latitudeDouble = double.Parse(latitude, CultureInfo.InvariantCulture);
            double longitudeDouble = double.Parse(longitude, CultureInfo.InvariantCulture); 
            return await search.findNearEventFromAPoint(latitudeDouble, longitudeDouble, int.Parse(radius), time, time.AddHours(8), httpContext);
        }

        /// <summary>
        /// Return a certain event with a specific facebook id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/event/{fbId}")]
        [PlaceToBeAuthenticationFilter]
        public async Task<Event> Get(string fbId)
        {            
            return await search.getEventByFbIdAsync(fbId); 
        }
    }
}
