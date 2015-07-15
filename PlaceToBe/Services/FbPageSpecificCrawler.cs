using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using placeToBe.Model;
using Newtonsoft.Json;

namespace placeToBe.Services
{
    public class FbPageSpecificCrawler : FbCrawler {
        private EventRepository eventRepo = new EventRepository();
        private PageRepository pageRepo = new PageRepository();

        public async Task findNewEventsOnPagesInDb() {
            List<string> eventsToFetch = new List<string>();

            //small version of an Event (Light Event) containing just the necessary data fields.
            Dictionary<String, Object> projectionContent = new Dictionary<string, object>() {
                {"fbId", 1}
            };
            ProjectionDefinition<Page, Page> projDefinition = new BsonDocument(projectionContent);

            var collection = pageRepo.GetCollection();

            using (var cursor = await collection.Find(new BsonDocument()).Project(projDefinition).ToCursorAsync()) {
                while (await cursor.MoveNextAsync()) {
                    var batch = cursor.Current;

                    var parallelOptions = new ParallelOptions {MaxDegreeOfParallelism = 10};
                    Parallel.ForEach(batch, parallelOptions,
                        page => {
                            parsePageAndCollectEventsToFetch(page.fbId, eventsToFetch);
                        });
                }

            }

            fetchEventsFromEventIdList(eventsToFetch);
        }

        private void parsePageAndCollectEventsToFetch(string pageId, List<string> eventsToFetch) {
            try {
                var page = fetchAndStorePage(pageId).Result;
                var response = graphApiGet(pageId, "searchEvent");
                var results =
                    completePaging(JsonConvert.DeserializeObject<FacebookPageResults>(response));

                foreach (var e in results) {
                    var dbE = eventRepo.GetByFbIdAsync(e.fbId).Result;
                    if (dbE == null && e.attending_count > 5) {
                        eventsToFetch.Add(e.fbId);
                        page.eventCount++;
                    }
                }
                pageRepo.UpdateAsync(page).Wait();
            }
            catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        //fetchAndStoreEvents
        private void fetchEventsFromEventIdList(List<string> eventsToFetch) {

            var eventfetchParallelOptions = new ParallelOptions {MaxDegreeOfParallelism = 10};
            //do parallel crawling with MaxDegreeOfParallelism(Value) threads
            Parallel.ForEach(eventsToFetch, eventfetchParallelOptions,
                e => { fetchAndStoreEvent(e).Wait(); });
        }


        public async Task<bool> addPageToDbBasedOnEventId(string eventId) {
            try {
                var e = JsonConvert.DeserializeObject<Event>(graphApiGet(eventId, "searchEventData"));
                List<string> eventsToFetch = new List<string>();
                parsePageAndCollectEventsToFetch(e.owner.id, eventsToFetch);
                fetchEventsFromEventIdList(eventsToFetch);
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }
    }
}