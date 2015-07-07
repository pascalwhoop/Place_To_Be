using MongoDB.Driver;
using placeToBe.Model.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            public User GetByEmailAsync(string email)
            {
                try {
                    var filter = Builders<User>.Filter.Eq("email", email);
                    return  _collection.Find(filter).FirstOrDefaultAsync().Result;
                }
                catch (Exception e) {
                    Debug.WriteLine(e);
                    return null;
                }
            }
            public async Task<User> GetByActivationCode(string activationcode)
            {
                var filter = Builders<User>.Filter.Eq("activationcode", activationcode);
                return await _collection.Find(filter).FirstOrDefaultAsync();
            }
    }
}