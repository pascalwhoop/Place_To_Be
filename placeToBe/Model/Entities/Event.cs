using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;
using placeToBe.Model.Entities;

namespace placeToBe.Model
{
    [DataContract]
    public class Event : EntityBase
    {
        public string description { get; set; }
        public bool is_date_only { get; set; }
        public string name { get; set; }
        public Owner owner { get; set; }
        public string privacy { get; set; }
        public string start_time { get; set; }
        public string timezone { get; set; }
        public string updated_time { get; set; }
        
        
        [DataMember(Name = "id")]
        public string fbId { get; set; }
        public List<Rsvp> attending { get; set; }
        public List<Rsvp> maybe { get; set; }

        public int attendingMale { get; set; }

        public int attendingFemale { get; set; }

        public int attendingCount { get; set; }

        public CoverPhoto cover { get; set; }

        public Page place { get; set; }

        public Venue venue { get; set; }

        public Category[] categoryList { get; set;  }

        public Location location { get; set;}
    }

    public struct Location
    {
        public string type { get; set; }
        public double[] coordinates { get; set; }

    }

    public class CoverPhoto
    {
        public string id { get; set; }
        public string source { get; set; }
        public int offset_x { get; set; }
        public int offset_y { get; set; }
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