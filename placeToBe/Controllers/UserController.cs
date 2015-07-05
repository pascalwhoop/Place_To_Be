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
        
        public async Task Post([FromUri]string userEmail, [FromUri] string userPassword)
        {
            await user.SendActivationEmail(userEmail, userPassword);
        }

        /// <summary>
        /// GET- Confirm user by activationcode.
        /// </summary>
        /// <param name="activationcode"></param>
        /// <returns></returns>
        public async Task Get([FromUri] string activationcode)
        {
            await user.ConfirmEmail(activationcode);
        }

        /// <summary>
        /// PUT- Login - After LoginBasicAuthentication 
        /// send a cookie to the user to stay logged in 
        /// or log out if the cookie is expired.
        /// </summary>
        /// <param name="resetPasswordByMail"></param>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        [LoginBasicAuthenticationFilter]
        public async Task<Cookie> Put([FromUri] string userEmail)
        {
            return await user.Login(userEmail);
        }

        /// <summary>
        /// PUT- Reset the old password and send a new one to the email.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public async Task Put([FromUri]string userEmail, string ueberladung)
        {
            await user.ForgetPasswordReset(userEmail);
        }

        /// <summary>
        /// PUT- Change the password from user from old password to the new one.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task Put([FromUri]string userEmail, [FromUri] string oldPassword, [FromUri] string newPassword)
        {
            await user.ChangePasswort(userEmail, oldPassword, newPassword);
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
        

    }
}
