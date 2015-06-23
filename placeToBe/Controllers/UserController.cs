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
using System.Security.Cryptography;
using System.Web.Security;

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
        public async Task<User> Get([FromUri]Guid id)
        {
            User User = await repo.GetByIdAsync(id);
            return User;
        }

        /* POST api/User
        
         */
        public async Task<FormsAuthenticationTicket> Put([FromUri]string loginmail, [FromUri]string loginpw, [FromUri] string ueberladung)
        {
            return await user.Login(loginmail, loginpw);
        }

        public void Put([FromUri]string senden){
            user.SendActivationEmail(senden);
        }

        //Register a User with email and passwort
        public async Task<Guid> Put([FromUri]string email, [FromUri] string passwort)
        {
            return await user.Register(email, passwort);
        }

        //public async Task<User> Put([FromUri]string email)
        //{
        //    UserRepository user = new UserRepository();
        //    return await user.GetByEmailAsync(email);
        //}

        // DELETE api/User/5
        //ToDo
        public async void Delete(Guid id)
        {
            var User = await repo.GetByIdAsync(id);
            repo.DeleteAsync(User);
        }
    }
}
