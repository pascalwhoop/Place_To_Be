using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;

namespace placeToBe.Model
{

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
        
        [BsonElement("Id")]
        public string fbId { get; set; }
        public List<Rsvp> attending { get; set; }
        public List<Rsvp> maybe { get; set; }

        public int attendingMale { get; set; }

        public int attendingFemale { get; set; }

        public int attendingCount { get; set; }

        public CoverPhoto cover { get; set; }

        public Page place { get; set; }


    }

    public class CoverPhoto
    {

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