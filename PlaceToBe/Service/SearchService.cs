using placeToBe.Model;
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
        MongoDbRepository<Event> repo = new MongoDbRepository<Event>();
        public void HeatSearch()
        {

        }

        public async void TextSearch(String filter)
        {
          IList<Event> list = await repo.SearchForAsync(filter);
        }

        public void EventSearch()
        {

        }

        public void FriendSearch()
        {

        }
    }
}