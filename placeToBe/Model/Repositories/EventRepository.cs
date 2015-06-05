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

        //a constructor that makes sure we have a geospherical index over our event list. 
        public EventRepository() {
            _collection.Indexes.CreateOneAsync(Builders<Event>.IndexKeys.Geo2DSphere(_ => _.locationCoordinates));
        }

        public async Task<List<LightEvent>> GetCityMapEvents(double [,]polygon, string time)
        {
            
            //filter request by location and time
            var filter = Builders<Event>.Filter.GeoWithinPolygon("locationCoordinates", polygon) & Builders<Event>.Filter.And(Builders<Event>.Filter.Gte("start_time",time),
                Builders<Event>.Filter.Lte("end_time", time));
            //search in db with filter
            IList<Event> eventList=await _collection.Find(filter).ToListAsync();

            List<LightEvent> lightList = new List<LightEvent>();
            //Convert Event to Lightevent
            foreach (Event _event in eventList)
            {
                LightEvent light = new LightEvent();
                light.Id = _event.Id;
                light.name = _event.name;
                light.locationCoordinates = _event.locationCoordinates;
                light.attendingCount = _event.attendingCount;

                lightList.Add(light);
            }
            return lightList;
        }

    }
}