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
using placeToBe.Filter;

namespace placeToBe.Controllers
{

    public class UserController : ApiController
    {

        MongoDbRepository<User> repo = new MongoDbRepository<User>();
        AccountService user = new AccountService();
        
        /// <summary>
        /// PUT- Send an activationemail and register a user with email and password (with inactive status)
        /// into the database.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        
        public async Task<HttpStatusCode> Post([FromUri]string userEmail, [FromUri] string userPassword)
        {
            return await user.SendActivationEmail(userEmail, userPassword);
        }

        /// <summary>
        /// GET- Confirm user by activationcode.
        /// </summary>
        /// <param name="activationcode"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> Get([FromUri] string activationcode)
        {
           return await user.ConfirmEmail(activationcode);
        }

        /// <summary>
        /// PUT- Reset the old password and send a new one to the email.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> Put([FromUri]string userEmail)
        {
           return await user.ForgetPasswordReset(userEmail);
        }

        /// <summary>
        /// PUT- Change the password from user from old password to the new one.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        [LoginBasicAuthenticationFilter]
        public async Task<HttpStatusCode> Put([FromUri]string userEmail, [FromUri] string oldPassword, [FromUri] string newPassword)
        {
           return await user.ChangePasswort(userEmail, oldPassword, newPassword);
        }

        /// <summary>
        /// POST- Save the JSON-object of a facebook-User in our database.
        /// </summary>
        /// <param name="fbuser"></param>
        /// <returns></returns>
        public async Task Post(FbUser fbuser)
        {
            await user.SaveFBData(fbuser);
        }

        // Methode dient nur zum Testen von Filtern.
        [LoginBasicAuthenticationFilter]
        public HttpStatusCode Put()
        {
            return HttpStatusCode.OK;
        }
        

    }
}
