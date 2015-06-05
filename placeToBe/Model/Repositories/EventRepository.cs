using placeToBe.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver.Core;

namespace placeToBe.Model.Repositories
{
    public class EventRepository: MongoDbRepository<Event>
    {

        public IMongoCollection<Event> EventCollection;
        private City test1;

        //a constructor that makes sure we have a geospherical index over our event list. 
        public EventRepository() {
            _collection.Indexes.CreateOneAsync(Builders<Event>.IndexKeys.Geo2DSphere(_ => _.locationCoordinates));
        }

        public async Task<IList<City>> GetCityMap(string place, string time)
        {
            
            var collection = GetCollection(); 
            var query = Builders<City>.Filter.GeoWithinPolygon("Location", test1.polygon);
            //var result = await collection.FindAsync(query);
            
            return null;
        }
       
        public IMongoCollection<Event> GetCollection()
        {
            return _collection;
        } 
    


    }
}