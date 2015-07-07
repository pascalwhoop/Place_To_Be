using Newtonsoft.Json;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using placeToBe.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace placeToBe.Filter
{
    public class FacebookUserVerification
    {

        /// <summary>
        /// Checks if the users request is authorized by proofing users access token:
        /// 1. Check if it equals the users access token in the Database.
        /// 2. If not... inspecting access tokens (user and app access token) via Facebooks Graph API
        /// </summary>
        /// <param name="userAccessToken">The users access token which has to be validated</param>
        /// <param name="userPassword">The users Facebook-Id</param>
        /// <returns>true if access token is validated, otherwise false</returns>

        private readonly String fbAppSecret = "469300d9c3ed9fe6ff4144d025bc1148";
        private readonly String fbAppId = "857640940981214";
        FbUserRepository fbUserRepo = new FbUserRepository();

        public async Task<bool> authorizeRequest(String userFbId, String shortAccessToken)
        {

            FbUser user = await fbUserRepo.GetByFbIdAsync(userFbId);

            if (user != null && shortAccessToken == user.shortAccessToken)
                return true;
            else
            {
                String appAccessToken = UtilService.performGetRequest(new Uri("https://graph.facebook.com/oauth/access_token?client_id=" + fbAppId + "&client_secret=" +
                              fbAppSecret + "&grant_type=client_credentials"));

                String jsonResponse = UtilService.performGetRequest(new Uri("https://graph.facebook.com/v2.3/debug_token?input_token=" + shortAccessToken + "&access_token=" + appAccessToken));

                Inspection insp = JsonConvert.DeserializeObject<Inspection>(jsonResponse);

                if (insp.is_valid == true)
                {
                    user.shortAccessToken = shortAccessToken;
                    await fbUserRepo.UpdateAsync(user);
                    return true;
                }
                else
                    return false;
            }
        }
    }
}