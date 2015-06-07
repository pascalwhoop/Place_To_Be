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
            _collection.Indexes.CreateOneAsync(Builders<Event>.IndexKeys.Geo2DSphere(_ => _.geoLocationCoordinates)); //an index on the location attribute
            _collection.Indexes.CreateOneAsync(Builders<Event>.IndexKeys.Descending(_ => _.startDateTime)); //an index on the startTime attribute
        }

        public async Task<List<LightEvent>> GetCityMapEvents(double [,]polygon, string time)
        {
            
            //filter request by location and time
            var filter = Builders<Event>.Filter.GeoWithinPolygon("geoLocationCoordinates", polygon) & Builders<Event>.Filter.And(Builders<Event>.Filter.Gte("start_time",time),
                Builders<Event>.Filter.Lte("end_time", time));
            //search in db with filter
            IList<Event> eventList=await _collection.Find(filter).ToListAsync();

            List<LightEvent> lightList = new List<LightEvent>();
            //Convert Event to Lightevent
            foreach (Event _event in eventList)
            {
                lightList.Add(eventToLightEvent(_event));
            }
            return lightList;
        }

        //TODO delete, only for testing purposes
        public async Task<List<LightEvent>> getSoonEvents(string time) {
            var max = Double.Parse(time);
            var filter = Builders<Event>.Filter.Gte("startDateTime", new DateTime());
            List<LightEvent> list = new List<LightEvent>();
            var dataList = await _collection.Find(filter).ToListAsync();
                   
                        foreach (Event e in dataList) {
                            list.Add(eventToLightEvent(e));
                        }
                    
                return list;
            }
        

        private LightEvent eventToLightEvent(Event e)
        {
                LightEvent light = new LightEvent();
                light.Id = e.Id;
                light.name = e.name;
                light.geoLocationCoordinates = e.geoLocationCoordinates;
                light.attendingCount = e.attendingCount;
            return light;
        }

    }
}