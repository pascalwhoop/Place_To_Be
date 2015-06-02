using placeToBe.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace placeToBe.Model.Repositories
{
    public class EventRepository<Event>: MongoDbRepository<Event>
    {
        public void GetCityMap(City city)
        {
            
        }
    }
}