using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace placeToBe.Model.Entities
{
    public class Gender : EntityBase
    {
        public Gender(string json)
        {
            JObject jGender = JObject.Parse(json);
            name = (string)jGender["name"];
            gender = (string)jGender["gender"];
            probability = (float)jGender["probability"];
            count = (int)jGender["count"];
        }

        public string name { get; set; }
        public string gender { get; set; }
        public float probability { get; set; }
        public int count { get; set; }
    }
}