using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using placeToBe.Model;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;

namespace placeToBe.Services
{
    public class FbEventSpecificCrawler : FbCrawler {
        private EventRepository eventRepo = new EventRepository();

        /// <summary>
        /// gets all events in db and iterates over them fetching all 
        /// </summary>
        /// <returns></returns>
        public async Task updateAllEventsInDb() {

            //creating a filter for the mongoDB event search: only events in the future are to be updated
            var builder = Builders<Event>.Filter;
            var filter = builder.Gte("startDateTime", DateTime.Now);

            //small version of an Event (Light Event) containing just the necessary data fields.
            Dictionary<String, Object> projectionContent = new Dictionary<string, object>() {
                {"fbId", 1}
            };
            ProjectionDefinition<Event, LightEvent> projDefinition = new BsonDocument(projectionContent);

            var collection = eventRepo.GetCollection();

            using (var cursor = await collection.Find(filter).Project(projDefinition).ToCursorAsync()) {
                while (await cursor.MoveNextAsync()) {
                    var batch = cursor.Current;

                    var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 10 };
                    //do parallel crawling with MaxDegreeOfParallelism(Value) threads
                    Parallel.ForEach(batch, parallelOptions,
                        e =>
                        {
                            try {
                                fetchAndStoreEvent(e.fbId).Wait();
                            }
                            catch (ArgumentNullException nex) {
                                Debug.WriteLine("nullpointer exception thrown");
                            }
                        });
                }
            }


        }

        
    }
}