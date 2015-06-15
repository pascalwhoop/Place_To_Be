using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;

namespace placeToBe.Model.Entities
{
    [DataContract]
    [BsonIgnoreExtraElements]
    public class LightEvent :EntityBase
    {
        [DataMember]
        public string name { get; set; }
        [DataMember(Name = "attending_count")]
        public int attendingCount { get; set; }
        [DataMember]
        public GeoLocation geoLocationCoordinates { get; set; }

        [DataMember(Name = "id")]
        public string fbId { get; set; }
    }
}