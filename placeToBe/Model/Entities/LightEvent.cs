using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;

namespace placeToBe.Model.Entities
{
    /// <summary>
    /// LightEvent represents a projection of the Event class.
    /// Sometimes retrieving all data belonging to an event object would mean a large overhead,
    /// because only a few specified attributes are needed. Therefore LightEvent represents a smaller version,
    /// containing just the events name, attendingCount, geo location and Facebook id.
    /// </summary>
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