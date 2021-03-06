﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace placeToBe.Model.Entities
{
    /// <summary>
    /// Facebook user class which contains the same fields as the 
    /// Facebook user JSON objects that we retrieve from Facebooks Graph API.
    /// </summary>
    public class FbUser : EntityBase
    {
        [JsonProperty("id")]
        public string fbId { get; set; }
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
        [JsonProperty("friends")]
        public Friends friends { get; set; }
        public string shortAccessToken { get; set; }
        public string longAccessToken { get; set; }
    }
    public class Datum
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class Summary
    {
        public int total_count { get; set; }
    }

    /// <summary>
    /// Friends of Facebook User who also use placeToBe.
    /// </summary>
    public class Friends
    {
        public List<Datum> data { get; set; }
        public Paging paging { get; set; }
        public Summary summary { get; set; }
    }
}