using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;


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
        protected static IMongoDatabase _database;
        protected IMongoCollection<TEntity> _collection;
        protected static MongoClient _client;

        public MongoDbRepository()
        {
            ConnectDatabase();
            setCollection();

        }

        public IMongoCollection<TEntity> GetCollection() {
            return _collection;
        } 

        public virtual async Task<Guid> InsertAsync(TEntity entity)
        {
            //if already exists, we update instead.
            if (entity.Id != Guid.Empty) {
                return await UpdateAsync(entity);
            }

            try {
                entity.Id = Guid.NewGuid();
                entity.lastUpdatedTimestamp = DateTime.Now;
                Task task = _collection.InsertOneAsync(entity);
                task.Wait();
                return entity.Id;
            }
            catch (Exception ex) {
                Debug.WriteLine(ex);
                return Guid.Empty;
            }
        }

        public async Task<Guid> UpdateAsync(TEntity entity) {
            entity.lastUpdatedTimestamp = DateTime.Now;
            var filter = Builders<TEntity>.Filter.Eq("_id", entity.Id);
            await _collection.ReplaceOneAsync(filter, entity);
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

        public virtual async Task<IList<TEntity>> GetAllAsync() {
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }

        public  Task<TEntity> GetByIdAsync(Guid id)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            return  _collection.Find(filter).FirstOrDefaultAsync();
        }

        public Task<TEntity> GetByNameAsync(String name)
        {
            var filter = Builders<TEntity>.Filter.Eq("name", name);
            return _collection.Find(filter).FirstOrDefaultAsync();
        }

        public  Task<TEntity> GetByFbIdAsync(String fbId)
        {
            var filter = Builders<TEntity>.Filter.Eq("fbId", fbId);
            return _collection.Find(filter).FirstOrDefaultAsync();
        }


        #region Private Helper Methods

        private void resetConnection() {
            ConnectDatabase();
            setCollection();
        }
        private void ConnectDatabase()
        {
            _client = new MongoClient(ConfigurationManager.AppSettings.Get("MongoDBConnectionString"));
            _database = _client.GetDatabase(ConfigurationManager.AppSettings.Get("MongoDBDatabaseName"));
        }



        private void setCollection()
        {
            _collection = _database
                .GetCollection<TEntity>(typeof(TEntity).Name);
            
        }
        #endregion
    }
}