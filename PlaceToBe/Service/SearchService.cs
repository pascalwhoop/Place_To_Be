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
    public class SearchService {

        private City city;
        private EventRepository repo = new EventRepository();
        public async Task<List<LightEvent>> HeatSearch(string place, DateTime startTime, DateTime endTime ){
            //Methode muss bearbeitet werden, sodass bestimmte Events abgerufen werden empfangen wird

            //TODO for now we mock for just cologne
            var colognePoly =  new double[5, 2] {
                {51.08496299999999, 6.7725819}, {51.08496299999999, 7.1620628}, {50.8295269, 7.1620628},
                {50.8295269, 6.7725819}, {51.08496299999999, 6.7725819}
            };
            return await repo.getEventsByTimeAndPolygon(colognePoly, startTime, endTime);
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