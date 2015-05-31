﻿using placeToBe.Model;
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
        public void HeatSearch(double latitude, double longitude)
        {

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