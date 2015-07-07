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
        [JsonProperty("start_time")]
        public String start_time { get; set; }

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

        

     
}