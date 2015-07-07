using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using placeToBe.Filter;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;

namespace placeToBe.Services {
    public class PlaceToBeAuthenticationFilter : BasicAuthenticationFilter {
        private readonly AccountService acc = new AccountService();
        private readonly UserRepository userRepo = new UserRepository();
        private readonly FacebookUserVerification fbVerification = new FacebookUserVerification();
        public PlaceToBeAuthenticationFilter() {}

        public PlaceToBeAuthenticationFilter(bool active) : base(active) {
            active = true;
        }

        //overriding method
        protected override bool OnAuthorizeUser(string userEmail, string userPassword, HttpActionContext actionContext) {
           
            // Authorization header malformed or not present.
            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userPassword)) {
                //return if fb auth works.                
                return tryAuthorizeAsFbUser(actionContext.Request.Headers);
            };            

            //contains Authorization header --> perform checkPassword
            return CheckPassword(userEmail, userPassword).Result;
        }

        protected bool tryAuthorizeAsFbUser(HttpRequestHeaders headers) {
            //see if we have a header FbAuthorization and if so get values from it
            IEnumerable<String> headerValues;
            if (!headers.TryGetValues("FbAuthorization", out headerValues)) return false;
            var fbAuthHeader = headerValues.FirstOrDefault();

            //convert from base64 to normal text
            fbAuthHeader = Encoding.Default.GetString(Convert.FromBase64String(fbAuthHeader));

            //if not correct number --> return
            var tokens = fbAuthHeader.Split(':');
            if (tokens.Length != 2) return false;

            //check for fb user id / token combo
            return fbVerification.authorizeRequest(tokens[0], tokens[1]).Result;
                
        }



        private async Task<bool> CheckPassword(string userEmail, string userPassword) {
            try {
                var userPasswordInBytes = Encoding.UTF8.GetBytes(userPassword);
                var user = await userRepo.GetByEmailAsync(userEmail);
                var salt = user.salt;
                var passwordSalt = acc.GenerateSaltedHash(userPasswordInBytes, salt);
                var comparePasswords = acc.CompareByteArrays(passwordSalt, user.passwordSalt);

                //statement: if users password is correct and status is activated          
                if (comparePasswords && user.status) return true;
                if (comparePasswords && user.status == false) {
                    //Please activate your acc.
                    return false;
                }
                //Authentification failed.
                return false;
            }
            catch (Exception e) {
                //Something go wrong.
                return false;
            }
        }
    }
}