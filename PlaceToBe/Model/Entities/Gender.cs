using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace placeToBe.Model.Entities
{
    public class Gender
    {
        public Gender(string json)
        {
            JObject jObject = JObject.Parse(json);
            JToken jGender = jObject["user"];
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