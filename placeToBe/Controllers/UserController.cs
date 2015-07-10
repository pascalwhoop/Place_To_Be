using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using placeToBe.Model.Entities;
using placeToBe.Services;

namespace placeToBe.Controllers
{
    public class UserController : ApiController
    {
        private readonly AccountService accountService = new AccountService();

        /// <summary>
        ///    PUT- Send an activationemail and register a accountService with email and passwort
        /// </summary>
        /// <param name="user">Post a user to DB</param>
        /// <returns>JsonResponse</returns>
        [System.Web.Http.Route("api/user/")]
        public async Task<JsonResponse> Post(User user)
        {
            try
            {
                await accountService.createUser(user);
                HttpContext.Current.Response.StatusCode = (int) HttpStatusCode.OK;
                return new JsonResponse
                {
                    status = "OK",
                    message = "User created successfully.",
                    showUser = true
                };
            }
            catch (AggregateException)
            {
                HttpContext.Current.Response.StatusCode = (int) HttpStatusCode.Conflict;
                return new JsonResponse
                {
                    status = "Error",
                    message = "Email already in use.",
                    showUser = true
                };
            }
            catch (Exception e)
            {
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return new JsonResponse
                {
                    status = "Error",
                    message = e.ToString(),
                    showUser = false
                };
            }
        }

        /// <summary>
        ///     GET- Activate the user by activationcode.
        /// </summary>
        /// <param name="activationcode">Get Request with activationcode</param>
        /// <returns>JsonResponse</returns>
        [System.Web.Http.Route("api/user/")]
        public async Task<JsonResponse> Get([FromUri] string activationcode)
        {
            try
            {
                await accountService.ConfirmEmail(activationcode);
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.OK;
                return new JsonResponse
                {
                    status = "OK",
                    message = "Account activated.",
                    showUser = true
                };
            }
            catch (Exception e)
            {
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return new JsonResponse
                {
                    status = "Error",
                    message = e.ToString(),
                    showUser = false
                };
            }
        }

        /// <summary>   
        /// PUT- Login - Get AuthenticationTicket for 5 minutes
        /// </summary>
        /// <param name="resetPasswordByMail"></param>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        /*
        public async Task<Cookie> Put([FromUri] string userEmail) {
            return await accountService.Login(userEmail);
        }*/

        /// <summary>
        ///     PUT- Reset the old password and send a new one to the email.
        /// </summary>
        /// <param name="userEmail">userEmail has to be a string</param>
        /// <returns></returns>
        [System.Web.Http.Route("api/user/{userEmail}/password_reset")]
        public async Task<JsonResponse> Post([FromUri] string userEmail)
        {
            try
            {

                await accountService.forgetPasswordReset(userEmail);
                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.OK;
                return new JsonResponse
                {
                    status = "OK",
                    message = "Password reset successful. Check your mails.",
                    showUser = true
                };
            }
            catch (Exception e)
            {

                HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return new JsonResponse
                {
                    status = "Error",
                    message = e.ToString(),
                    showUser = false
                };
            }

        }

        /// <summary>
        /// Change the password from a User
        /// </summary>
        /// <param name="pcp">Contains an email, an old password and a newPassword as a string</param>
        /// <returns>JsonResponse</returns>
        [PlaceToBeAuthenticationFilter]
        [System.Web.Http.Route("api/user/{userEmail}/password_change")]
        public async Task<JsonResponse> Put(PasswordChangePair pcp)
        {

            var code = await accountService.changePassword(pcp.email, pcp.oldPassword, pcp.newPassword);
            HttpContext.Current.Response.StatusCode = (int)code;
            switch (code)
            {
                case HttpStatusCode.OK:
                    return new JsonResponse
                    {
                        status = "OK",
                        message = "Password changed successfully.",
                        showUser = true
                    };
                case HttpStatusCode.BadRequest:
                    return new JsonResponse
                    {
                        status = "Error",
                        message = "False password.",
                        showUser = true
                    };
                case HttpStatusCode.Conflict:
                    return new JsonResponse
                    {
                        status = "Error",
                        message = "Database timeout.",
                        showUser = false
                    };
                case HttpStatusCode.NotFound:
                    return new JsonResponse
                    {
                        status = "Error",
                        message = "User not found.",
                        showUser = false
                    };
            }
            return new JsonResponse
            {
                status = "Error",
                message = "Error occured.",
                showUser = false
            };


        }

        /// <summary>
        /// Authorize a user
        /// </summary>
        /// <returns></returns>
        [PlaceToBeAuthenticationFilter]
        [System.Web.Http.Route("api/user/authorize")]
        public async Task<ActionResult> Get()
        {
            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }
    }
}