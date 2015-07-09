using MongoDB.Driver;

namespace placeToBe.Model.Repositories
{
    /// <summary>
    /// A repository to get access to all the saved Facebook pages in the MongoDb and therefore be able to modify them.
    /// </summary>
    public class PageRepository: MongoDbRepository<Page>
    {
        /// <summary>
        /// a constructor that makes sure we have an index on all our users emails.
        /// </summary>
        public PageRepository() {
            //unique index on fb pages id
            CreateIndexOptions options = new CreateIndexOptions {Unique = true};
            _collection.Indexes.CreateOneAsync(Builders<Page>.IndexKeys.Ascending(_ => _.fbId), options);           
        }
    }
}