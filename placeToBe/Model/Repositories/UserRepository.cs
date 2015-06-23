using MongoDB.Driver;
using placeToBe.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace placeToBe.Model.Repositories
{
    public class UserRepository:MongoDbRepository<User>
    {
            public UserRepository() {
            //unique index on email of a user
            CreateIndexOptions options = new CreateIndexOptions {Unique = true};
            _collection.Indexes.CreateOneAsync(Builders<User>.IndexKeys.Text(_ => _.email), options);
            
        }

            public async Task<User> GetByEmailAsync(String email)
            {
                var filter = Builders<User>.Filter.Eq("email", email);
                return await _collection.Find(filter).FirstOrDefaultAsync();
            }
    }
}