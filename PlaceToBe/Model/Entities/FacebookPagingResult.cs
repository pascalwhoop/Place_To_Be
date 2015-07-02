using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace placeToBe.Model.Entities
{
    public class FacebookPagingResult
    {
          
        [JsonProperty("id")]
        public string fbId { get; set; }
            
        [JsonProperty("is_community_page")]
        public bool is_community_page {get;set;}

        [JsonProperty("attending_count")]
        public int attending_count { get; set; }
        [JsonProperty("name")]
        public String name { get; set; }
        [JsonProperty("rsvp_status")]
        public String rsvp_status { get; set; }

        }

        public class Paging
        {

            [JsonProperty("next")]
            public string next { get; set; }
        }

        public class FacebookPageResults
        {

            [JsonProperty("data")]
            public FacebookPagingResult[] data { get; set; }

            [JsonProperty("paging")]
            public Paging paging { get; set; }
        }

        public class Metadata
        {
            [JsonProperty("sso")]
            public string sso { get; set; }
        }

        public class Inspection
        {
            [JsonProperty("app_id")]
            public long app_id { get; set; }
            [JsonProperty("application")]
            public string application { get; set; }
            [JsonProperty("expires_at")]
            public int expires_at { get; set; }
            [JsonProperty("is_valid")]
            public bool is_valid { get; set; }
            [JsonProperty("issued_at")]
            public int issued_at { get; set; }
            [JsonProperty("metadata")]
            public Metadata metadata { get; set; }
            [JsonProperty("scopes")]
            public List<string> scopes { get; set; }
            [JsonProperty("user_id")]
            public int user_id { get; set; }
        } 
}