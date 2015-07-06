using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;

namespace placeToBe.Controllers
{
    public class CityController : ApiController
    {

        readonly CityRepository cityRepo = new CityRepository();
        
        /// <summary>
        /// Return a list of the cities in the database.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<City>> Get() {
                    return await cityRepo.GetAllAsync();
                } 
    }
}
