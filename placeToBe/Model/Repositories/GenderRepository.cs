using MongoDB.Driver;
using placeToBe.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace placeToBe.Model.Repositories
{
    /// <summary>
    /// A repository to get access to all the saved names (and their corresponding genders) in the MongoDb and therefore be able to modify them.
    /// </summary>
    public class GenderRepository: MongoDbRepository<Gender>
    {
            /// <summary>
            /// a constructor that makes sure we have an index on all our names in the MongoDb gender collection.
            /// </summary>
            public GenderRepository() {
            //unique index on name
            CreateIndexOptions options = new CreateIndexOptions {Unique = true};    
            _collection.Indexes.CreateOneAsync(Builders<User>.IndexKeys.Ascending(_ => _.name), options)
            
        }
    }
}