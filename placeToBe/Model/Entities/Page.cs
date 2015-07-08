using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;
using placeToBe.Model.Entities;

namespace placeToBe.Model{

    /// <summary>
    /// Facebook Page class which contains the same fields as the 
    /// Facebook Page JSON objects that we retrieve from Facebooks Graph API.
    /// </summary>
    [DataContract]
    public class Page  : EntityBase
    {
        [DataMember(Name = "id")]
        public string fbId { get; set; }
        [DataMember]
        public string about { get; set; }
        [DataMember]
        public string attire { get; set; }
        [DataMember]
        public bool can_post { get; set; }
        [DataMember]
        public string category { get; set; }
        [DataMember]
        public List<CategoryList> category_list { get; set; }
        [DataMember]
        public int checkins { get; set; }
        [DataMember]
        public Cover cover { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public bool has_added_app { get; set; }
        [DataMember]
        public Hours hours { get; set; }
        [DataMember]
        public bool is_community_page { get; set; }
        [DataMember]
        public bool is_published { get; set; }
        [DataMember]
        public int likes { get; set; }
        [DataMember]
        public string link { get; set; }
        [DataMember]
        public FbLocation location { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public Parking parking { get; set; }
        [DataMember]
        public PaymentOptions payment_options { get; set; }
        [DataMember]
        public string phone { get; set; }
        [DataMember]
        public string public_transit { get; set; }
        [DataMember]
        public RestaurantServices restaurant_services { get; set; }
        [DataMember]
        public RestaurantSpecialties restaurant_specialties { get; set; }
        [DataMember]
        public int talking_about_count { get; set; }
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public string website { get; set; }
        [DataMember]
        public int were_here_count { get; set; }
    }

    public class CategoryList
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Cover
    {
        public string cover_id { get; set; }
        public int offset_x { get; set; }
        public int offset_y { get; set; }
        public string source { get; set; }
        public string id { get; set; }
    }

    public class Hours
    {
        public string mon_1_open { get; set; }
        public string mon_1_close { get; set; }
        public string tue_1_open { get; set; }
        public string tue_1_close { get; set; }
        public string wed_1_open { get; set; }
        public string wed_1_close { get; set; }
        public string thu_1_open { get; set; }
        public string thu_1_close { get; set; }
        public string fri_1_open { get; set; }
        public string fri_1_close { get; set; }
        public string sat_1_open { get; set; }
        public string sat_1_close { get; set; }
        public string fri_2_open { get; set; }
        public string fri_2_close { get; set; }
    }

                       

    public class Parking
    {
        public int lot { get; set; }
        public int street { get; set; }
        public int valet { get; set; }
    }

    public class PaymentOptions
    {
        public int amex { get; set; }
        public int cash_only { get; set; }
        public int discover { get; set; }
        public int mastercard { get; set; }
        public int visa { get; set; }
    }

    public class RestaurantServices
    {
        public int delivery { get; set; }
        public int catering { get; set; }
        public int groups { get; set; }
        public int kids { get; set; }
        public int outdoor { get; set; }
        public int reserve { get; set; }
        public int takeout { get; set; }
        public int waiter { get; set; }
        public int walkins { get; set; }
    }

    public class RestaurantSpecialties
    {
        public int breakfast { get; set; }
        public int coffee { get; set; }
        public int dinner { get; set; }
        public int drinks { get; set; }
        public int lunch { get; set; }
    }
}