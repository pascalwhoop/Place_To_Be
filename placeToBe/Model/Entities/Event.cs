using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace placeToBe.Model.Entities

{
    /// <summary>
    /// Facebook Event class which contains the same fields as the 
    /// Facebook Event JSON objects that we retrieve from Facebooks Graph API.
    /// </summary>
    [DataContract]
    public class Event : EntityBase
    {
        //mapping between different fieldnames we have to make a DataContract to deal with the objects 
        //which are retrieved by placeToBes REST API.
        [DataMember(Name = "id")]
        public string fbId { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public bool is_date_only { get; set; }
        [DataMember]
        public Owner owner { get; set; }
        [DataMember]
        public string privacy { get; set; }
        [DataMember]
        public string start_time { get; set; }
        public DateTime startDateTime { get; set; }
        [DataMember]
        public string end_time { get; set; }
        public DateTime endDateTime { get; set; }
        [DataMember]
        public string timezone { get; set; }
        [DataMember]
        public string updated_time { get; set; }      

        [DataMember]
        public int invitedCount { get; set; }
        [DataMember]
        public List<Rsvp> attending { get; set; }
        [DataMember]
        public List<Rsvp> maybe { get; set; }
        //Analysed by GenderizeService
        [DataMember]
        public int attendingMale { get; set; }
        //Analysed by GenderizeService
        [DataMember]
        public int attendingFemale { get; set; }
        [DataMember]
        //Analysed by GenderizeService
        public int attendingUndefined { get; set; }
        [DataMember]
        public CoverPhoto cover { get; set; }
        [DataMember]
        public Place place { get; set; }
        public Category[] categoryList { get; set;  }
        [DataMember]
        public string name { get; set; }
        [DataMember(Name = "attending_count")]
        public int attendingCount { get; set; }
        [DataMember]
        public GeoLocation geoLocationCoordinates { get; set; }

    }
    /// <summary>
    /// starting v2.3 this is the necessary (and we have to ask for it specifically) 
    /// part of an Event that describes the location (and therefore latLng information)
    /// </summary>
    [DataContract]
    public class Place
    {
        [DataMember]
        public string name { get; set; }
        [DataMember(Name = "location")]
        public FbLocation location { get; set; }
        [DataMember]
        public string id { get; set; }
    }

    public class FbLocation {
        public string city { get; set; }
        public string country { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string street { get; set; }
        public string zip { get; set; }
    }
    /// <summary>
    /// GeoLocation point where we can put an index on later (MongoDB Geo2DSphere)
    /// </summary>
    [DataContract]
    public class GeoLocation
    {  
        public GeoLocation(double lat, double lng) {
            this.coordinates = new double[2]{lat, lng};
            type = "Point";
        }
        public string type { get; set; }
        [DataMember]
        public double[] coordinates { get; set; }
    }
    public class CoverPhoto
    {
        public string id { get; set; }
        public string source { get; set; }
        public float offset_x { get; set; }
        public float offset_y { get; set; }
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

    public class Category
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}