using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace placeToBe.Model.Entities

{
    [DataContract]
    public class Event : EntityBase
    {
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public bool is_date_only { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public Owner owner { get; set; }
        [DataMember]
        public string privacy { get; set; }
        [DataMember]
        public string start_time { get; set; }
        [DataMember]
        public string end_time { get; set; }
        [DataMember]
        public string timezone { get; set; }
        [DataMember]
        public string updated_time { get; set; }      
        [DataMember(Name = "id")]
        public string fbId { get; set; }
        [DataMember]
        public int invitedCount { get; set; }
        [DataMember]
        public List<Rsvp> attending { get; set; }
        [DataMember]
        public List<Rsvp> maybe { get; set; }
        [DataMember]
        public int attendingMale { get; set; }
        [DataMember]
        public int attendingFemale { get; set; }
        [DataMember]
        public int attendingCount { get; set; }
        [DataMember]
        public CoverPhoto cover { get; set; }
        [DataMember]
        public Page place { get; set; }
        [DataMember]
        public Venue venue { get; set; }
        [DataMember]
        public Location location { get; set;}
        [DataMember]
        public Category[] categoryList { get; set;  }

    }

    public class Location
    {
        public string type { get; set; }
        public Coordinates coordinates { get; set; }
    }
    public class CoverPhoto
    {
        public string id { get; set; }
        public string source { get; set; }
        public float offset_x { get; set; }
        public float offset_y { get; set; }
    }

    public class Venue
    {
        public string city { get; set; }
        public string country { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string street{ get; set; }
        public string zip { get; set; }
        public string id { get; set; }
    } 


    public class Owner
    {
        public string category { get; set; }
        public string name { get; set; }
        public string id { get; set; }
    }

    public class Rsvp
    {
        public string name { get; set; }
        public string rsvp_status { get; set; }
        public string id { get; set; }
    }
}