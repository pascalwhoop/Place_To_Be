using MongoDB.Driver;
using placeToBe.Model.Entities;

namespace placeToBe.Model.Repositories
{
    /// <summary>
    /// A repository to get access to all the saved Facebook users in the MongoDb and therefore be able to modify them.
    /// </summary>
    public class FbUserRepository:MongoDbRepository<FbUser>
    {
        /// <summary>
        /// a constructor that makes sure we have a facebook id index over our FbUsers list. 
        /// </summary>
        public FbUserRepository() {
            //unique index on fb pages id
            CreateIndexOptions options = new CreateIndexOptions {Unique = true};
            _collection.Indexes.CreateOneAsync(Builders<FbUser>.IndexKeys.Text(_ => _.fbId), options);          
        }
    }
}