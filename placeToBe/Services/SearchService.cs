using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;

namespace placeToBe.Services
{
    public class SearchService
    {

        private EventRepository eventRepo = new EventRepository();
        private CityRepository cityRepo = new CityRepository();
        private FbUserRepository fbUserRepo = new FbUserRepository();
        UtilService utilService = new UtilService();
        public List<Datum> data { get; set; }

        /// <summary>
        /// Get a list of LightEvents from the EventRepository.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public async Task<List<LightEvent>> HeatSearch(string id, DateTime startTime, DateTime endTime)
        {
            var city = await cityRepo.getByPlaceId(id);
            return await eventRepo.getEventsByTimeAndPolygon(city.getPolygon(), startTime, endTime);
        }

        /// <summary>
        /// TODO: Text search.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IList<Event>> TextSearch(String filter)
        {
            IList<Event> list = await eventRepo.SearchForAsync(filter);
            return list;
        }

        /// <summary>
        /// Return a certain event by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Event> EventSearch(Guid id)
        {
            Event _event = await eventRepo.GetByIdAsync(id);
            return _event;
        }

        /// <summary>
        /// Find a near event from a certain point on the map.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="radius"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public async Task<List<Event>> findNearEventFromAPoint(double latitude, double longitude, int radius, DateTime startTime, DateTime endTime, HttpContext httpContext)
        {
            //get fbId from header of request
            String fbId = utilService.getFbIdFromHttpContext(httpContext);

            List<Event> nearEvents = await eventRepo.getFullEventListByPointInRadius(latitude, longitude, radius, startTime, endTime);

            //Only for fb-Users
            //fill friends who attend the event
            if (fbId != null)
            {
                FbUser fbUser = await fbUserRepo.GetByFbIdAsync(fbId);
                for (int i = 0; i < nearEvents.Count; i++)
                {
                    nearEvents[i] = await getEventAttendingFriends(fbUser, nearEvents[i]);
                    nearEvents[i].attending = null;
                }
            }

            return nearEvents;
        }

        /// <summary>
        /// Compare FbUser friends with people attending the event to find out if friends of the FbUser attend at the event
        /// </summary>
        /// <param name="fbUser">Facebook User who have done the request</param>
        /// <param name="currentEvent">the requestet event</param>
        /// <returns>updated event with the friends attending at the event</returns>
        public async Task<Event> getEventAttendingFriends(FbUser fbUser, Event currentEvent)
        {
            List<FbUser> eventAttendingFriends = new List<FbUser>();
            List<Rsvp> eventAttendingPeople = currentEvent.attending;
            List<Datum> fbUserFriends = fbUser.friends.data;

 
            if (eventAttendingPeople == null || fbUserFriends==null)
            {
                return currentEvent;
            }
            for (int i = 0; i < fbUserFriends.Count; i++)
            {
                for (int j = 0; j < eventAttendingPeople.Count; j++)
                {
                    if (fbUserFriends.ElementAt(i).id == eventAttendingPeople.ElementAt(j).id)
                        eventAttendingFriends.Add(await fbUserRepo.GetByFbIdAsync(fbUserFriends.ElementAt(i).id));
                }
            }
            //store the who attend the event to event
            currentEvent.attendingFriends = eventAttendingFriends;
            return currentEvent;
        }

    }
}
