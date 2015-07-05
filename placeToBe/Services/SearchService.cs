using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;

namespace placeToBe.Services
{
    public class SearchService {

        private EventRepository eventRepo = new EventRepository();
        private CityRepository cityRepo = new CityRepository();

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
        public async Task<List<Event>> findNearEventFromAPoint(double latitude, double longitude, int radius, DateTime startTime, DateTime endTime)
        {
            return await eventRepo.getFullEventListByPointInRadius(latitude, longitude, radius, startTime, endTime);
        } 

        /// <summary>
        /// TODO: Facebook FriendSearch
        /// </summary>
        public void FriendSearch()
        {

        }
    }
}