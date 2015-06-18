using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using System.Threading.Tasks;
using placeToBe.Services;

namespace placeToBe.Controllers
{
    public class UserController : ApiController
    {

        MongoDbRepository<User> repo = new MongoDbRepository<User>();
        AccountService user = new AccountService();

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

        /* POST api/User
        
         */
        public async Task<bool> LoginPost(string email, string password)
        {
             return await user.Login(email, password);
        }

       //Register user - start service and put in db
        public void RegisterPost(string email, string password)
        {
          user.Register(email, password);
        }

        // PUT api/User/5
        public void Put(int id, [FromBody]string value)
        {
            // Put...
        }

        // DELETE api/User/5
        //ToDo
        public async void Delete(Guid id)
        {
            var User = await repo.GetByIdAsync(id);
            repo.DeleteAsync(User);
        }
    }
}
