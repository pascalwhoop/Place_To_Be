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
        public async Task<List<LightEvent>> HeatSearch(Guid cityGuid, DateTime startTime, DateTime endTime ){
            //Methode muss bearbeitet werden, sodass bestimmte Events abgerufen werden empfangen wird

            var city = await cityRepo.GetByIdAsync(cityGuid);

            return await eventRepo.getEventsByTimeAndPolygon(city.getPolygon(), startTime, endTime);
        }

        public async Task<IList<Event>> TextSearch(String filter)
        {
            IList<Event> list = await eventRepo.SearchForAsync(filter);
            return list;
        }

        public async Task<Event> EventSearch(Guid id)
        {
            Event _event = await eventRepo.GetByIdAsync(id);
            return _event;
        }

        public void FriendSearch()
        {

        }
    }
}