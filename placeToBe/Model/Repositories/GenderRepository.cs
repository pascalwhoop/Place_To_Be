using MongoDB.Driver;
using placeToBe.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace placeToBe.Model.Repositories
{
    public class GenderRepository: MongoDbRepository<Gender>
    {
            public GenderRepository() {
            //unique index on name
            CreateIndexOptions options = new CreateIndexOptions {Unique = true};
            _collection.Indexes.CreateOneAsync(Builders<Gender>.IndexKeys.Text(_ => _.name), options);
            
        }
    }
}