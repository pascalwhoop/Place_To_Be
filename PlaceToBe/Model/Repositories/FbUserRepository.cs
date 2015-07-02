using MongoDB.Driver;
using placeToBe.Model.Entities;

namespace placeToBe.Model.Repositories
{
    public class FbUserRepository:MongoDbRepository<FbUser>
    {
        //a constructor that makes sure we have a facebook id index over our FbUsers list. 
        public FbUserRepository() {
            //unique index on fb pages id
            CreateIndexOptions options = new CreateIndexOptions {Unique = true};
            _collection.Indexes.CreateOneAsync(Builders<FbUser>.IndexKeys.Text(_ => _.fbId), options);          
        }
    }
}