using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace placeToBe.Model.Repositories {
    /// <summary>
    /// A MongoDB repository. Maps to a collection with the same name
    /// as type TEntity.
    /// </summary>
    /// <typeparam name="TEntity">Entity type for this repository</typeparam>
    public class MongoDbRepository<TEntity> : 
        IRepository<TEntity> where 
            TEntity : EntityBase
    {
        protected IMongoDatabase _database;
        protected IMongoCollection<TEntity> _collection;

        public MongoDbRepository()
        {
            GetDatabase();
            setCollection();

        }

        public IMongoCollection<TEntity> GetCollection() {
            return _collection;
        } 

        public async Task<Guid> InsertAsync(TEntity entity)
        {
            entity.Id = Guid.NewGuid();
            await _collection.InsertOneAsync(entity);
            return entity.Id;

        }

        public async Task<Guid> UpdateAsync(TEntity entity)
        {
                await _collection.InsertOneAsync(entity);
            return entity.Id;
        }

        public async Task<Guid> DeleteAsync(TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", entity.Id);
            await _collection.DeleteOneAsync(filter);
            return entity.Id;
            
        }

        public async Task<IList<TEntity>> SearchForAsync(string filterText) {
            var filter = Builders<TEntity>.Filter.Text(filterText);
            return await _collection.Find(filter).ToListAsync();

        }

        public async Task<IList<TEntity>> GetAllAsync() {
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<TEntity> GetByIdAsync(String name)
        {
            var filter = Builders<TEntity>.Filter.Eq("name", name);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        #region Private Helper Methods
        private void GetDatabase()
        {
            var client = new MongoClient(GetConnectionString());
            _database = client.GetDatabase(GetDatabaseName());
        }

        private string GetConnectionString()
        {
            return ConfigurationManager
                .AppSettings
                .Get("MongoDBConnectionString")
                .Replace("{DB_NAME}", GetDatabaseName());
        }

        private string GetDatabaseName()
        {
            return ConfigurationManager
                .AppSettings
                .Get("MongoDBDatabaseName");
        }


        private void setCollection()
        {
            _collection = _database
                .GetCollection<TEntity>(typeof(TEntity).Name);
            
        }
        #endregion
    }
}