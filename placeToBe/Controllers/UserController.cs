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

        //PUT- Send an activationemail and register a user with email and passwort
        public async Task Post([FromUri]string email, [FromUri] string password)
        {
            await user.SendActivationEmail(email, password);
        }

        //GET- user by activationcode for confirm mail. 
        public async Task Get([FromUri] string activationcode)
        {
            await user.ConfirmEmail(activationcode);
        }

        // PUT- Login - Get AuthenticationTicket for 5 minutes

        public async Task<FormsAuthenticationTicket> Put([FromUri]string email, [FromUri]string password)
        {
            return await user.Login(email, password);
        }

        //Reset the old password and send a new one to the email.
        public async Task Put([FromUri]string email)
        {
            await user.ForgetPasswordReset(email);
        }

        //Change the password from the user. 
        public async Task Put([FromUri]string email, [FromUri] string oldpassword, [FromUri] string newpassword)
        {
            await user.ChangePasswort(email, oldpassword, newpassword);
        }

    }
}
