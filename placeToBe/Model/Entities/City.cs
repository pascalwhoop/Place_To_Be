using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;

namespace placeToBe.Model.Entities
{
    /// <summary>
    /// class includes a City represented by its name and coordinates 
    /// (coordinates build quader which includes the city).
    /// </summary>
    public class City : EntityBase
    {
        
        public String name { get; set; }
        public Point northWest { get; set; }
        Point northEast { get; set; }
        Point southEast { get; set; }
        Point southWest { get; set; }

    }
    public class Point {

        public double dx { get; set; }
        public double dy { get; set; }

        public Point(double dx, double dy){
            this.dx = dx;
            this.dy = dy;
         }
    }
}