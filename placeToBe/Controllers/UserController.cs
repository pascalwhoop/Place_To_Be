using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace placeToBe.Controllers
{
    public class UserController : Controller
    {
            /*
             * Transfer User-data
             * 
            MongoDbRepository<User> repo = new MongoDbRepository<User>();
        
            // GET api/User
            public async Task<IList<User>>Get()
            {
                IList<User> list = await repo.GetAllAsync();
                return list;
            }

            // GET api/User/5
            public async Task<User> Get(Guid id)
            {
                User user = await repo.GetByIdAsync(id);
                return user;
            }

            // POST api/User
            public void Post([FromBody]User user)
            {
                repo.InsertAsync(user);
            }

            // PUT api/User/5
            public void Put(int id, [FromBody]User user)
            {
                repo.UpdateAsync(User);
            }

            // DELETE api/User/5
            public void Delete(int id)
            {
                User user = await repo.GetByIdAsync(id);
                repo.DeleteAsync(User);
            }
             * 
             * 
             * */
        }
    }
}