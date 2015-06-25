using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using placeToBe.Model.Entities;

namespace placeToBe.Model.Repositories
{
    public class CityRepository: MongoDbRepository<City>
    {
        
        public CityRepository() {        
            CreateIndexOptions options = new CreateIndexOptions {Unique = true};
            _collection.Indexes.CreateOneAsync(Builders<City>.IndexKeys.Text(_ => _.place_id), options);
        }


        //only return a light version of our cities. Because it will only be a short list, we dont perform a complicated DB side projection but rather just create a new list of objects
        public async override Task<IList<City>> GetAllAsync() {
           var cities = await base.GetAllAsync();
            var lightCities = cities.Select(city => new City() {
                formatted_address = city.formatted_address, place_id = city.place_id, geometry = city.geometry
            }).ToList();
            return lightCities;
        }

        public async Task<City> getByPlaceId(string placeId) {
            
        }
    }

    
}