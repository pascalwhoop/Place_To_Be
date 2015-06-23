using System;
using System.Reflection.Emit;
using System.Runtime.Serialization.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe;
using placeToBe.Model;
using placeToBe.Service;
using placeToBe.Model.Entities;


namespace placeToBe.Tests.Service
{
    [TestClass]
    public class FbCrawlerTest
    {
        [TestMethod]
        public void Shuffle()
        {
            //Arrange
            FbCrawler crawler = new FbCrawler();


            Coordinates test1 = new Coordinates(23,25);
            Coordinates test2 = new Coordinates(0, 25);
            Coordinates test3 = new Coordinates(3, 25);
            

            
            Coordinates[] testor =    { test1, test2, test3 };
            Coordinates[] expected1 = { test1, test2, test3 };
            Coordinates[] expected2 = { test1, test3, test2 }; 
            Coordinates[] expected3 = { test2, test1, test3 };
            Coordinates[] expected4 = { test2, test3, test1 }; 
            Coordinates[] expected5 = { test3, test1, test2 };
            Coordinates[] expected6 = { test3, test2, test1 }; 

            
            //Act
            Coordinates[] result = crawler.shuffle(testor);

            //Assert
            if (result == expected1)
            {
                Assert.AreEqual(result, expected1);
            }
            else if (result == expected2)
            {
                Assert.AreEqual(result, expected1);
            }
            else if (result == expected3)
            {
                Assert.AreEqual(result, expected2);
            }
            else if (result == expected4)
            {
                Assert.AreEqual(result, expected4);
            }
            else if (result == expected5)
            {
                Assert.AreEqual(result, expected5);
            }
            else if (result == expected6)
            {
                Assert.AreEqual(result, expected6);
            }
            
        }

        [TestMethod]
        public void HandlePlace() {
            FbCrawler crawler = new FbCrawler();
            var place =
                "{\"id\":\"150350494981623\",\"can_post\":false,\"category\":\"Hotel\",\"category_list\":[{\"id\":\"164243073639257\",\"name\":\"Hotel\"},{\"id\":\"273819889375819\",\"name\":\"Restaurant\"},{\"id\":\"110290705711626\",\"name\":\"Bar\"}],\"checkins\":1434,\"cover\":{\"cover_id\":\"1086126261404037\",\"offset_x\":0,\"offset_y\":0,\"source\":\"https:\\/\\/scontent.xx.fbcdn.net\\/hphotos-xfa1\\/v\\/t1.0-9\\/s720x720\\/10308054_1086126261404037_3460166067659233902_n.jpg?oh=38131c7bbab83987955e70dce85de589&oe=55FCDA15\",\"id\":\"1086126261404037\"},\"description\":\"Tradition kombiniert mit moderner Gastronomie & gehobener K\\u00fcche\\n\\nVerwendet werden selbstverst\\u00e4ndlich nur Rohstoffe von ausgew\\u00e4hlten und erstklassigen Erzeugern. Mit dieser Einstellung gewinnen die Br\\u00fcder Wilbrand sowie ihr K\\u00fcchen-Team immer neue Freunde.\",\"general_info\":\"Der MICHELIN F\\u00fchrer 2013 k\\u00fcrte unser Team zum 9. mal mit 1 Stern (Gourmet Restaurant) und einem Bib Gourmand (Postsch\\u00e4nke).\\nDer GAULT MILLAU 2012 hat unserer Mannschaft 16 Punkte verliehen f\\u00fcr hohe KREATIVIT\\u00c4T und QUALIT\\u00c4T.\",\"has_added_app\":false,\"is_community_page\":false,\"is_published\":true,\"likes\":2173,\"link\":\"https:\\/\\/www.facebook.com\\/pages\\/Hotel-Restaurant-Zur-Post-in-Odenthal\\/150350494981623\",\"location\":{\"city\":\"Odenthal\",\"country\":\"Germany\",\"latitude\":51.03245398355,\"longitude\":7.1154067667291,\"street\":\"Altenberger Dom Stra\\u00dfe 23\",\"zip\":\"51519\"},\"name\":\"Hotel & Restaurant \\\"Zur Post\\\" in Odenthal\",\"parking\":{\"lot\":1,\"street\":0,\"valet\":0},\"phone\":\"+49 2202 977780\",\"price_range\":\"$$$ (30-50)\",\"talking_about_count\":29,\"website\":\"http:\\/\\/www.zurpost.eu\",\"were_here_count\":1434}";
            var condition = "searchPlace";
            var id = "150350494981623";

            

            crawler.handlePlace(place, condition, id);
        }

        [TestMethod]
        public void FindEventsOnPageTest() {
            FbCrawler crawler = new FbCrawler();
            var placeID = "346376098748775";    //Reinecke Fuchs. Gibt aktuell noch Probleme (können irgendwie nicht auf dessen Events zugreifen
            placeID = "252874629056";           //Bootshaus. Da kommen wir irgendwie drauf... Altersbegrenzung??
            crawler.fetchEventsOnPage(placeID);
        }

        [TestMethod]
        public void FillEmptyEventFieldsTest() {
            FbCrawler crawler = new FbCrawler();
            Place place = new Place();
            Event fbEvent = new Event();

            place.name = "Bonnstr./ Ecke Aachenerstr. Haltestelle Weiden- West, 50226 Frechen";
            fbEvent.place = place;

            crawler.FillEmptyEventFields(fbEvent);

        }
    }
}
