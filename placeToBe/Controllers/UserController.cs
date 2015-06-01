using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using System.Threading.Tasks;

namespace placeToBe.Controllers
{
    public class UserController : ApiController
    {

        MongoDbRepository<User> repo = new MongoDbRepository<User>();

        // GET api/User 
        public async Task<IList<User>> Get()
        {
            IList<User> list = await repo.GetAllAsync();
            return list;
        }

        // GET api/User/5
        public async Task<User> Get(Guid id)
        {
            User User = await repo.GetByIdAsync(id);
            return User;
        }

        // POST api/User
        public void Post([FromBody]string value)
        {
            //  repo.InsertAsync(User);
        }

        // PUT api/User/5
        public void Put(int id, [FromBody]string value)
        {
            // Put...
        }

        // DELETE api/User/5
        public async void Delete(Guid id)
        {
            var User = await repo.GetByIdAsync(id);
            repo.DeleteAsync(User);
        }
    }
}
