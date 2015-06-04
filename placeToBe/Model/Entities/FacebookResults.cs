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
    
}