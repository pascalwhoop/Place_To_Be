using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace placeToBe.Model.Entities
{
    public class FacebookResults
    {
            [JsonProperty("id")]
            public string id { get; set; }

        }

        public class Paging
        {

            [JsonProperty("next")]
            public string next { get; set; }
        }

        public class FacebookPageResults
        {

            [JsonProperty("data")]
            public FacebookResults[] data { get; set; }

            [JsonProperty("paging")]
            public Paging paging { get; set; }
        }

        public class FacebookPageResultsAttending
        {
            [JsonProperty("data")]
            public ResultAttending[] data { get; set; }

            [JsonProperty("paging")]
            public Paging paging { get; set; }
        }

        public class ResultAttending
        {
            [JsonProperty("name")]
            public String name { get; set; }
            [JsonProperty("rsvp_status")]
            public String rsvp_status { get; set; }
            [JsonProperty("id")]
            public String id { get; set; }
        }
    
}