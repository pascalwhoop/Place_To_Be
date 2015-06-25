using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace placeToBe.Model.Entities {
    /// <summary>
    /// class includes a City represented by its name and coordinates 
    /// (coordinates build quader which includes the city).
    /// </summary>
    [DataContract]
    public class City : EntityBase {
        [DataMember]
        public List<AddressComponent> address_components { get; set; }

        [DataMember]
        public string formatted_address { get; set; }

        [DataMember]
        public Geometry geometry { get; set; }

        [DataMember]
        public string place_id { get; set; }

        [DataMember]
        public List<string> types { get; set; }

        //to have a way of checking the oldest cities first
        public DateTime lastCheckedTime { get; set; }

        //builds a polygon array as needed by mongodb
        public double[,] getPolygon() {
            var a = new double[5, 2];
            //northwest
            a[0, 0] = geometry.bounds.northeast.lat;
            a[0, 1] = geometry.bounds.southwest.lng;
            //northeast
            a[1, 0] = geometry.bounds.northeast.lat;
            a[1, 1] = geometry.bounds.northeast.lng;
            //southeast
            a[2, 0] = geometry.bounds.southwest.lat;
            a[2, 1] = geometry.bounds.northeast.lng;
            //southwest
            a[3, 0] = geometry.bounds.southwest.lat;
            a[3, 1] = geometry.bounds.southwest.lng;
            //northeast (again to make an enclosing form)
            a[0, 0] = geometry.bounds.northeast.lat;
            a[0, 1] = geometry.bounds.southwest.lng;
            return a;
        }
    }

    public class Coordinates {
        public Coordinates(double latitude, double longitude) {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    // ==========================================================00

    [DataContract]
    public class AddressComponent {
        [DataMember]
        public string long_name { get; set; }

        [DataMember]
        public string short_name { get; set; }

        [DataMember]
        public List<string> types { get; set; }
    }

    [DataContract]
    public class Northeast {
        [DataMember]
        public double lat { get; set; }

        [DataMember]
        public double lng { get; set; }
    }

    [DataContract]
    public class Southwest {
        [DataMember]
        public double lat { get; set; }

        [DataMember]
        public double lng { get; set; }
    }

    [DataContract]
    public class Bounds {
        [DataMember]
        public Northeast northeast { get; set; }

        [DataMember]
        public Southwest southwest { get; set; }
    }

    [DataContract]
    public class Location {
        [DataMember]
        public double lat { get; set; }

        [DataMember]
        public double lng { get; set; }
    }

    [DataContract]
    public class Northeast2 {
        [DataMember]
        public double lat { get; set; }

        [DataMember]
        public double lng { get; set; }
    }

    [DataContract]
    public class Southwest2 {
        [DataMember]
        public double lat { get; set; }

        [DataMember]
        public double lng { get; set; }
    }

    [DataContract]
    public class Viewport {
        [DataMember]
        public Northeast2 northeast { get; set; }

        [DataMember]
        public Southwest2 southwest { get; set; }
    }

    [DataContract]
    public class Geometry {
        [DataMember]
        public Bounds bounds { get; set; }

        [DataMember]
        public Location location { get; set; }

        [DataMember]
        public string location_type { get; set; }

        [DataMember]
        public Viewport viewport { get; set; }
    }

    [DataContract]
    public class GMapsGeocodingResponse {
        [DataMember]
        public List<City> results { get; set; }

        [DataMember]
        public string status { get; set; }
    }
}