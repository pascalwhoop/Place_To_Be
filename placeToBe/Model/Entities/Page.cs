using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace placeToBe.Model{

                       public class Page  : EntityBase
                       {
                           public string id { get; set; }
                           public string about { get; set; }
                           public string attire { get; set; }
                           public bool can_post { get; set; }
                           public string category { get; set; }
                           public List<CategoryList> category_list { get; set; }
                           public int checkins { get; set; }
                           public Cover cover { get; set; }
                           public string description { get; set; }
                           public bool has_added_app { get; set; }
                           public Hours hours { get; set; }
                           public bool is_community_page { get; set; }
                           public bool is_published { get; set; }
                           public int likes { get; set; }
                           public string link { get; set; }
                           public Location location { get; set; }
                           public string name { get; set; }
                           public Parking parking { get; set; }
                           public PaymentOptions payment_options { get; set; }
                           public string phone { get; set; }
                           public string public_transit { get; set; }
                           public RestaurantServices restaurant_services { get; set; }
                           public RestaurantSpecialties restaurant_specialties { get; set; }
                           public int talking_about_count { get; set; }
                           public string username { get; set; }
                           public string website { get; set; }
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

                       public class Location
                       {
                           public string city { get; set; }
                           public string country { get; set; }
                           public double latitude { get; set; }
                           public double longitude { get; set; }
                           public string street { get; set; }
                           public string zip { get; set; }
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