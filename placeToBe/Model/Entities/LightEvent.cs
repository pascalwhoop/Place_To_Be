using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace placeToBe.Model.Entities
{
    [DataContract]
    public class LightEvent :EntityBase
    {
        [DataMember]
        public string name { get; set; }
        [DataMember(Name = "attending_count")]
        public int attendingCount { get; set; }
        [DataMember]
        public GeoLocation geoLocationCoordinates { get; set; }
    }
}