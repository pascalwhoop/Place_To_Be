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
    /// <summary>
    /// A repository to get access to all the saved events in the MongoDb and therefore be able to modify them.
    /// </summary>
    public class EventRepository: MongoDbRepository<Event>
    {
        /// <summary>
        /// a constructor that makes sure we have a geospherical index over our event list aswell 
        /// as an index on the Facebook id and the starting time of an event. 
        /// </summary>
        public EventRepository() {
            //unique index on fb events
            CreateIndexOptions options = new CreateIndexOptions {Unique = true};
            _collection.Indexes.CreateOneAsync(Builders<Event>.IndexKeys.Text(_ => _.fbId), options);
            _collection.Indexes.CreateOneAsync(Builders<Event>.IndexKeys.Geo2DSphere(_ => _.geoLocationCoordinates)); //an index on the location attribute
            _collection.Indexes.CreateOneAsync(Builders<Event>.IndexKeys.Descending(_ => _.startDateTime)); //an index on the startTime attribute
        }

        /// <summary>
        /// Retrieving events from the database which are within a polygon (representing for example a specified part of a city)
        /// and between a defined time range.
        /// </summary>
        public async Task<List<LightEvent>> getEventsByTimeAndPolygon(double[,] polygon, DateTime startTime,
            DateTime endTime) {

            //creating a filter for the mongoDB event search
            var builder = Builders<Event>.Filter;
            var filter = builder.GeoWithinPolygon("geoLocationCoordinates", polygon) &
                         builder.Gte("startDateTime", startTime.AddHours(-4)) & builder.Lt("startDateTime", endTime);
            
            //small version of an Event (Light Event) containing just a few important data fields.
            Dictionary<String, Object> projectionContent = new Dictionary<string, object>() {
                {"attendingCount", 1},
                {"geoLocationCoordinates", 1},
                {"name", 1},
                {"fbId", 1}
            };

            //Search in event collection with the specified filter.
            ProjectionDefinition<Event,LightEvent > projDefinition = new BsonDocument(projectionContent);
            var task =  _collection.Find(filter).Project(projDefinition).ToListAsync();
            var events = task.Result;
            return events;
        }
        /// <summary>
        /// Retrieving a list of events which takes place within a specified radius around a coordinate (latitude/longitude)
        /// and between a defined time range.
        /// </summary>
        public async Task<List<Event>> getFullEventListByPointInRadius(double latitude, double longitude, int radius, DateTime startTime, DateTime endTime) {
            double maxDistance = radius; //Geolocation search requires double value.

            //Creating a GeoJson point which represents the center of the search circle.
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
                {"cover",1},
                {"place", 1},
                {"start_time",1},
                {"end_time",1},
                {"attendingMale",1},
                {"attendingFemale",1}
            };
            ProjectionDefinition<Event, Event> projDefinition = new BsonDocument(projectionContent);
            var task = _collection.Find(filter).Project(projDefinition).ToListAsync();
            var events = task.Result;
            return events;
        }
        /// <summary>
        /// Smaller version of the event class
        /// </summary>
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