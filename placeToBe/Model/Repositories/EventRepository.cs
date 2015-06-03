using placeToBe.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;

namespace placeToBe.Model.Repositories
{
    public class EventRepository: MongoDbRepository<Event>
    {
        //a constructor that makes sure we have a geospherical index over our event list. 
        public EventRepository() {
            _collection.Indexes.CreateOneAsync(Builders<Event>.IndexKeys.Geo2DSphere(_ => _.location));
        }

        public void GetCityMap(City city)
        {
            
        }
    }
}