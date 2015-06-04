using placeToBe.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;

namespace placeToBe.Model.Repositories
{
    public class PageRepository: MongoDbRepository<Page>
    {
        //a constructor that makes sure we have a facebook id index over our page list. 
        public PageRepository() {
            _collection.Indexes.CreateOneAsync(Builders<Page>.IndexKeys.Text(_ => _.fbId));
        }

    }
}