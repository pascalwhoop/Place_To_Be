using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace placeToBe.Services
{
    public class SearchService
    {

        City city;
        EventRepository repo = new EventRepository();
        public async Task<List<LightEvent>> HeatSearch(string place, string time) {
            city.name = place;
            //City.name einbauen ??
            //GetEvents from the city in polygon 
            List<LightEvent> allEvents = await repo.GetCityMapEvents(city.polygon, time);
            return allEvents;
        }

        public async Task<IList<Event>> TextSearch(String filter)
        {
            IList<Event> list = await repo.SearchForAsync(filter);
            return list;
        }

        public async Task<Event> EventSearch(Guid id)
        {
            Event _event = await repo.GetByIdAsync(id);
            return _event;
        }

        public void FriendSearch()
        {

        }
    }
}