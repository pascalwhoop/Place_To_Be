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
    /// A MongoDB repository. Maps to a collection with the same name as type TEntity.
    /// </summary>
    /// <typeparam name="TEntity">Entity type for this repository.</typeparam>
    public class MongoDbRepository<TEntity> : 
        IRepository<TEntity> where 
            TEntity : EntityBase
    {
        protected static IMongoDatabase _database;
        protected IMongoCollection<TEntity> _collection;
        protected static MongoClient _client;

        /// <summary>
        /// Constructor automatically connects Repository to the MongoDb and retrieves the needed collection.
        /// </summary>
        public MongoDbRepository()
        {
            ConnectDatabase();
            setCollection();
        }

        public IMongoCollection<TEntity> GetCollection() {
            return _collection;
        }

        /// <summary>
        /// Inserts an object as an Bson document in the collection and gives it a unique id. 
        /// If it already exists, it will be updated instead.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        /// <returns>True if the update was successful otherwise false.</returns>
        public virtual async Task<Guid> InsertAsync(TEntity entity)
        {
        
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
        /// <summary>
        /// Updates an entity that is already in the repository.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        /// <returns>True if the update was successful otherwise false.</returns>
        public async Task<Guid> UpdateAsync(TEntity entity) {
            entity.lastUpdatedTimestamp = DateTime.Now;
            var filter = Builders<TEntity>.Filter.Eq("_id", entity.Id);
            await _collection.ReplaceOneAsync(filter, entity);
            return entity.Id;
        }
        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="entity">Entity to remove.</param>
        /// <returns>True if an entity was deleted otherwise false.</returns>
        public async Task<Guid> DeleteAsync(TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", entity.Id);
            await _collection.DeleteOneAsync(filter);
            return entity.Id;
            
        }
        /// <summary>
        /// Searches for a list of entities that match a specified filter.
        /// </summary>
        /// <param name="filterText">filter to use when searching for entities.</param>
        /// <returns></returns>
        public async Task<IList<TEntity>> SearchForAsync(string filterText) {
            var filter = Builders<TEntity>.Filter.Text(filterText);
            return await _collection.Find(filter).ToListAsync();

        }
        /// <summary>
        /// Retrieves all the entities from the repository.
        /// </summary>
        /// <returns>List of the collections entities.</returns>
        public virtual async Task<IList<TEntity>> GetAllAsync() {
            return await _collection.Find(new BsonDocument()).ToListAsync();
        }
        /// <summary>
        /// Retrieves an entity by its globally unique id.
        /// </summary>
        /// <param name="id">Id of the entity to retrieve.</param>
        /// <returns>A matching entity with the specified id.</returns>
        public  Task<TEntity> GetByIdAsync(Guid id)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            return  _collection.Find(filter).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Retrieves an entity by its name (has to be an attributes name of the entity).
        /// </summary>
        /// <param name="name">Value of the name attribute of the entity to retrieve.</param>
        /// <returns>A matching entity with the specified name value.</returns>
        public Task<TEntity> GetByNameAsync(String name)
        {
            var filter = Builders<TEntity>.Filter.Eq("name", name);
            return _collection.Find(filter).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Retrieves an entity by its Facebook id.
        /// </summary>
        /// <param name="fbId">Facebook id of the entity to retrieve.</param>
        /// <returns>A matching entity with the specified name value.</returns>
        public  Task<TEntity> GetByFbIdAsync(String fbId)
        {
            var filter = Builders<TEntity>.Filter.Eq("fbId", fbId);
            return _collection.Find(filter).FirstOrDefaultAsync();
        }


        #region Private Helper Methods

        /// <summary>
        /// Resets the connection to the MongoDB.
        /// </summary>
        private void resetConnection() {
            ConnectDatabase();
            setCollection();
        }
        /// <summary>
        /// Connects repository to the MongoDb.
        /// </summary>
        private void ConnectDatabase()
        {
            _client = new MongoClient(ConfigurationManager.AppSettings.Get("MongoDBConnectionString"));
            _database = _client.GetDatabase(ConfigurationManager.AppSettings.Get("MongoDBDatabaseName"));
        }
        /// <summary>
        /// Specifies the right collection from the MongoDb for the repository.
        /// </summary>
        private void setCollection()
        {
            _collection = _database.GetCollection<TEntity>(typeof(TEntity).Name);     
        }
        #endregion
    }
}