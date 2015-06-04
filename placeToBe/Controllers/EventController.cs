using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using placeToBe.Services;
using System.Collections;

namespace placeToBe.Controllers
{
    public class EventController : ApiController
    {
        readonly SearchService search = new SearchService();
        readonly MongoDbRepository<Event> repo = new MongoDbRepository<Event>();
       
        /**
         * Diese Methode soll ein JSON Array von Events zurückgeben, welche für die Heatmap genutzt werden. 
         */

        [Route("api/event/filter/{place}/{time}")]
        public async Task<IList<Event>> getEventsByTimeAndPlace(string place, string time) {
            
            return await search.HeatSearch(place, time);
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
        /*
        // GET api/event/gettext
        // Gibt eine Liste von Text zurück.
        public async Task<IList<Event>> GetText(){
            IList<Event> searchText = await search.TextSearch(filter);
            return searchText ;
         }
        
         * */
        // GET api/event/Getfriends
        // Gibt eine Liste von Freunden zurück
        // public aysnch Task<IList<Friend>> GetFriend();

        // POST api/event

        public async Task<Guid> Post([FromBody]Event Event)
        {
           return await repo.InsertAsync(Event);
        }

        // PUT api/event/5
        public async Task<Guid> Put(Guid id, [FromBody]Event Event)
        {
            return await repo.UpdateAsync(Event);
        }

        // DELETE api/event/5
        public async void Delete(Guid id) {
            var Event = await repo.GetByIdAsync(id);
            repo.DeleteAsync(Event);
        }
    }
}
