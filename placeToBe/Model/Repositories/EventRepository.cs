using placeToBe.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver.Core;
using MongoDB.Driver.GeoJsonObjectModel;

namespace placeToBe.Model.Repositories
{
    public class EventRepository: MongoDbRepository<Event>
    {

        //a constructor that makes sure we have a geospherical index over our event list. 
        public EventRepository() {
            //unique index on fb events
            CreateIndexOptions options = new CreateIndexOptions {Unique = true};
            _collection.Indexes.CreateOneAsync(Builders<Event>.IndexKeys.Text(_ => _.fbId), options);
            _collection.Indexes.CreateOneAsync(Builders<Event>.IndexKeys.Geo2DSphere(_ => _.geoLocationCoordinates)); //an index on the location attribute
            _collection.Indexes.CreateOneAsync(Builders<Event>.IndexKeys.Descending(_ => _.startDateTime)); //an index on the startTime attribute
        }

       /* public async Task<List<LightEvent>> GetCityMapEvents(double [,]polygon, string time)
        {
            
            //filter request by location and time
            var filter = Builders<Event>.Filter.GeoWithinPolygon("geoLocationCoordinates", polygon) & Builders<Event>.Filter.And(Builders<Event>.Filter.Gte("startDateTime",new DateTime() ),
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
        }*/

        //help: http://mongodb.github.io/mongo-csharp-driver/2.0/reference/driver/definitions/#projections
        public async Task<List<LightEvent>> getEventsByTimeAndPolygon(double[,] polygon, DateTime startTime,
            DateTime endTime) {

            
            var builder = Builders<Event>.Filter;
            var filter = builder.GeoWithinPolygon("geoLocationCoordinates", polygon) &
                         builder.Gte("startDateTime", startTime.AddHours(-4)) & builder.Lt("startDateTime", endTime);
            
            Dictionary<String, Object> projectionContent = new Dictionary<string, object>() {
                {"attendingCount", 1},
                {"geoLocationCoordinates", 1},
                {"name", 1},
                {"fbId", 1}
            };
            ProjectionDefinition<Event,LightEvent > projDefinition = new BsonDocument(projectionContent);
            var task =  _collection.Find(filter).Project(projDefinition).ToListAsync();
            var events = task.Result;
            return events;

        }

        public async Task<List<Event>> getFullEventListByPointInRadius(double latitude, double longitude, DateTime startTime, DateTime endTime)
        {
            double maxDistance = 2000;//in meters
            //muss eine GeoJson point sein, wenn einfach latitude und longitude uebergeben werden dann sucht er im umkreis des lat und lng jeweils
            var point = GeoJson.Point(GeoJson.Geographic(latitude, longitude));
            var builder = Builders<Event>.Filter;
            var filter = builder.NearSphere("geoLocationCoordinates", point, maxDistance)
                & builder.Gte("startDateTime", startTime.AddHours(-4)) & builder.Lt("startDateTime", endTime);
            

            Dictionary<String, Object> projectionContent = new Dictionary<string, object>() {
                {"attendingCount", 1},
                {"geoLocationCoordinates", 1},
                {"name", 1},
                {"fbId", 1},
                {"description",1},
                {"coverPhoto",1},
                {"place", 1},
                {"start_time",1},
                {"end_time",1},
                {"attendMale",1},
                {"attendingFemale",1}
            };
            ProjectionDefinition<Event, Event> projDefinition = new BsonDocument(projectionContent);
            var task = _collection.Find(filter).Project(projDefinition).ToListAsync();
            var events = task.Result;
            return events;
        } 

        /*//TODO delete, only for testing purposes
        public async Task<List<LightEvent>> getSoonEvents(string time) {
            var max = Double.Parse(time);
            var filter = Builders<Event>.Filter.Gte("startDateTime", new DateTime());
            List<LightEvent> list = new List<LightEvent>();
            var dataList = await _collection.Find(filter).ToListAsync();
                   
                        foreach (Event e in dataList) {
                            list.Add(eventToLightEvent(e));
                        }
                    
                return list;
            }*/
        

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