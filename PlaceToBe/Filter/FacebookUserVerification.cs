using Newtonsoft.Json;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using placeToBe.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace placeToBe.Filter
{
    //author: Stephan Blumenthal

    /// <summary>
    /// Class containing methods to authenticate, save and or update a Facebook user.
    /// </summary>
    public class FacebookUserVerification
    {
        private readonly String fbAppSecret = ConfigurationManager.AppSettings["fbAppSecret"];
        private readonly String fbAppId = ConfigurationManager.AppSettings["fbAppId"];
        FbUserRepository fbUserRepo = new FbUserRepository();

        /// <summary>
        /// Checks if the users request is authenticated/authorized by proofing users access token:
        /// 1. Check if user exists in the database if not fetch the user from Fracebooks Graph API.
        /// 1.1. If user exists check if it is up to date (if not fetch user)
        /// 2. Check if it equals the users access token in the Database.
        /// 2.1. If not... inspecting access tokens (user and app access token) via Facebooks Graph API.
        /// </summary>
        /// <param name="shortAccessToken">The users access token which has to be validated</param>
        /// <param name="userPassword">The users Facebook-Id</param>
        /// <returns>true if access token is validated, otherwise false</returns>
        public async Task<bool> authorizeRequest(String userFbId, String shortAccessToken)
        {

            FbUser user = await fbUserRepo.GetByFbIdAsync(userFbId);

            //fetch user from FB if not yet in DB
            if (user == null) {
               user = await fetchAndStoreUserDetails(shortAccessToken);
            }
            //Update Fb User when last updated time > 60 minutes
            else
            {
                if((DateTime.Now - user.lastUpdatedTimestamp).TotalHours>1.00)
                    user = await fetchAndStoreUserDetails(shortAccessToken);
            }

            //When Users short access token is the same as the one in the Database then the user ist validated
            if (user != null && shortAccessToken == user.shortAccessToken)
                return true;
            else
            {
                String appAccessToken = UtilService.performGetRequest(new Uri("https://graph.facebook.com/oauth/access_token?client_id=" + fbAppId + "&client_secret=" +
                              fbAppSecret + "&grant_type=client_credentials"));

                String jsonResponse = UtilService.performGetRequest(new Uri("https://graph.facebook.com/v2.3/debug_token?input_token=" + shortAccessToken + "&" + appAccessToken));

                FbTokenInspection insp = JsonConvert.DeserializeObject<FbTokenInspection>(jsonResponse);

                if (insp.data.is_valid == true)
                {
                    if (user != null) {
                        user.shortAccessToken = shortAccessToken;
                        fbUserRepo.UpdateAsync(user);
                    }
                    return true;
                }
                else
                    return false;
            }
        }
        /// <summary>
        /// fetches Facebook user from Facebooks Graph API 
        /// </summary>
        /// <param name="shortAccessToken">The users access token which is needed to fetch the user from Facebook</param>
        /// <returns>the Facebook user</returns>
        private async Task<FbUser> fetchAndStoreUserDetails(string shortAccessToken) {
            String jsonResponse = UtilService.performGetRequest(
                new Uri(
                    "https://graph.facebook.com/v2.3/me?fields=id,email,first_name,last_name,gender,link,updated_time,verified,friends&access_token=" +
                    shortAccessToken));
            FbUser user = JsonConvert.DeserializeObject<FbUser>(jsonResponse);
            user.shortAccessToken = shortAccessToken;
            await fbUserRepo.InsertAsync(user);
            return user;
        }
    }
}