using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using placeToBe.Model;
using placeToBe.Model.Repositories;

namespace placeToBe.Controllers
{
    public class EventController : ApiController
    {

        MongoDbRepository<Event> repo = new MongoDbRepository<Event>();
 
        // GET api/event
        public async Task<IList<Event>> Get() {
            IList<Event> list = await repo.GetAllAsync();
            return list;
        }

        // GET api/event/5
        public async Task<Event> Get(Guid id)
        {
            Event Event = await repo.GetByIdAsync(id);
            return Event;
        }

        // POST api/event
        public void Post([FromBody]Event Event)
        {
           repo.InsertAsync(Event);
        }

        // PUT api/event/5
        public void Put(Guid id, [FromBody]Event Event)
        {
            repo.UpdateAsync(Event);
        }

        // DELETE api/event/5
        public async void Delete(Guid id) {
            var Event = await repo.GetByIdAsync(id);
            repo.DeleteAsync(Event);
        }
    }
}
