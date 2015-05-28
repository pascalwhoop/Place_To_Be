using placeToBe.Model;
using placeToBe.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace placeToBe.Services
{
    public class SearchService
    {
        MongoDbRepository<Event> repo = new MongoDbRepository<Event>();

        // GET api/event
        public async Task<IList<Event>> Get()
        {
            IList<Event> list = await repo.GetAllAsync();
            return list;
        }

        public async Task<IList<Event>> Get(String filter)
        {
            IList<Event> list = await repo.SearchForAsync(filter);
            return list;
        }

        // GET api/event/5
        public async Task<Event> Get(Guid id)
        {
            Event Event = await repo.GetByIdAsync(id);
            return Event;
        }
    }
}