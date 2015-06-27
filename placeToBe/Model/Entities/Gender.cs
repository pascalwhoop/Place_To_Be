using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace placeToBe.Model.Entities
{
     [DataContract]
    public class Gender : EntityBase
    {
         [DataMember]
        public string name { get; set; }
         [DataMember]
        public string gender { get; set; }
         [DataMember]
        public float probability { get; set; }
         [DataMember]
        public int count { get; set; }
    }
}