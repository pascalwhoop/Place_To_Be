using System;
using System.Collections.Generic;
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
       
        /**
         * Diese Methode soll ein JSON Array von Events zurückgeben, welche für die Heatmap genutzt werden. 
         */

        [Route("api/event/filter/{id}/{year}/{month}/{day}/{hour}")]
        public async Task<List<LightEvent>> getEventsByTimeAndCity(string id, string year, string month, string day, string hour)
        {

            DateTime time = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), 0, 0);
            return await search.HeatSearch(id, time, time.AddHours(8));
        } 

        // GET api/event/5
        // Gibt ein bestimmtes Event zurück
        public async Task<Event> GetEvent(Guid id)
        {
            
            Event setEvent = await search.EventSearch(id);
            return setEvent;
           
            /*
             * Pascal Code - direkter MongoDb Zugriff
             * Event Event = await repo.GetByIdAsync(id);
            return Event;
             * */
        }
    }
}
