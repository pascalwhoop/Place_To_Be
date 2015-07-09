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
            /// <summary>
            /// a constructor that makes sure we have a user email index over our users list.
            /// </summary>
            public UserRepository() {
            //unique index on email of a user            
            CreateIndexOptions options = new CreateIndexOptions {Unique = true};    
            _collection.Indexes.CreateOneAsync(Builders<User>.IndexKeys.Ascending(_ => _.email), options)
            
        }
            /// <summary>
            /// Retrieves an user by his/her email address.
            /// </summary>
            /// <param name="id">Email address of the user to retrieve.</param>
            /// <returns>A matching User with the specified email adress.</returns>
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
            /// <summary>
            /// Retrieves an user by his/her activationcode.
            /// </summary>
            /// <param name="id">Activationcode of the user to retrieve.</param>
            /// <returns>A matching User with the specified activationcode.</returns>
            public async Task<User> GetByActivationCode(string activationcode)
            {
                var filter = Builders<User>.Filter.Eq("activationcode", activationcode);
                return await _collection.Find(filter).FirstOrDefaultAsync();
            }
    }
}