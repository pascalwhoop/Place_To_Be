using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using placeToBe.Services;
using System.Web.Http;

namespace placeToBe.Controllers
{
    public class CityController : ApiController
    {

        readonly CityRepository cityRepo = new CityRepository();
        



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [PlaceToBeAuthenticationFilter]
        public async Task<IList<City>> Get() {
                    return await cityRepo.GetAllAsync();
                } 
    }
}
