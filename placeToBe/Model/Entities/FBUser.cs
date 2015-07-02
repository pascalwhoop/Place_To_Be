using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace placeToBe.Model.Entities
{
    public class FBUser : User
    {
        [JsonProperty("id")]
        public int FB_ID { get; set; }
        [JsonProperty("email")]
        public string emailFB { get; set; }
        [JsonProperty("first_name")]
        public string firstName { get; set; }
        [JsonProperty("last_name")]
        public string lastName { get; set; }
        [JsonProperty("name")]
        public string nickname { get; set; }
        [JsonProperty("gender")]
        public string gender { get; set; }
        [JsonProperty("link")]
        public string httpLink { get; set; }
        [JsonProperty("locale")]
        public string country { get; set; }
        [JsonProperty("timezone")]
        public int timezone { get; set; }
        [JsonProperty("updated_time")]
        public string updatedTimeFB { get; set; }
        [JsonProperty("verified")]
        public bool verified { get; set; }

    }
}