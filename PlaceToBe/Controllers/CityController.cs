using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using placeToBe.Services;

namespace placeToBe.Controllers
{
    public class CityController : ApiController
    {

        readonly CityRepository cityRepo = new CityRepository();

        [PlaceToBeAuthenticationFilter]
        public async Task<IList<City>> Get() {
                    return await cityRepo.GetAllAsync();
                } 
    }
}
