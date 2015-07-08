using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using placeToBe.Model.Entities;

namespace placeToBe.Model.Repositories
{
    /// <summary>
    /// A repository to get access to all the saved cities in the MongoDb and therefore be able to modify them.
    /// </summary>
    public class CityRepository: MongoDbRepository<City>
    {
        /// <summary>
        /// An index will be put on the place_id field by calling the default constructor.
        /// </summary>
        public CityRepository() {        
            CreateIndexOptions options = new CreateIndexOptions {Unique = true};
            _collection.Indexes.CreateOneAsync(Builders<City>.IndexKeys.Text(_ => _.place_id), options);
        }


        /// <summary>
        /// This Method only returns a light version of our cities. Because it will only be a short list, 
        /// we dont perform a complicated DB side projection but rather just create a new list of objects.
        /// </summary>
        public async override Task<IList<City>> GetAllAsync() {
           var cities = await base.GetAllAsync();
            var lightCities = cities.Select(city => new City() {
                formatted_address = city.formatted_address, place_id = city.place_id, geometry = city.geometry
            }).ToList();
            return lightCities;
        }

        /// <summary>
        /// Finds a City by its placeId and returns it when it exists.
        /// </summary>
        public async Task<City> getByPlaceId(string placeId) {
            var filter = Builders<City>.Filter.Eq("place_id", placeId);
            return await _collection.Find(filter).FirstOrDefaultAsync(); 
        }
    } 
}