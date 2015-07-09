using System;
using System.Reflection.Emit;
using System.Runtime.Serialization.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe;
using placeToBe.Model;
using placeToBe.Services;
using placeToBe.Model.Entities;


namespace placeToBe.Tests.Service
{
    //author: Merve Bas 

    /// <summary>
    /// Class is testing methods of the class FbCrawler
    /// Procedure: 
    /// creating dummy- parameters and passing it over 
    /// </summary>
    [TestClass]
    public class FbCrawlerTest
    {
        /// <summary>
        /// Testing the method shuffle of class FbCrawler 
        /// Procedure: 
        /// crating dummy- parameters und comparing the expected results with the actual result of the method
        /// </summary>
        [TestMethod]
        public void shuffle()
        {
            //Arrange
            FbCrawler crawler = new FbCrawler();


            Coordinates test1 = new Coordinates(23, 25);
            Coordinates test2 = new Coordinates(0, 25);
            Coordinates test3 = new Coordinates(3, 25);



            Coordinates[] testor = { test1, test2, test3 };
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


        /// <summary>
        /// Testing the method findEventsOnPage of class FbCrawler 
        /// author: author of the method indEventsOnPage
        /// </summary>
        [TestMethod]
        public void findEventsOnPageTest()
        {
            FbCrawler crawler = new FbCrawler();
            var placeID = "346376098748775";    
            placeID = "252874629056";           
            crawler.fetchEventsOnPage(placeID);
        }

        /// <summary>
        /// Testing the method fillEmptyEventFieldsTest of class FbCrawler 
        /// author: author of the method fillEmptyEventFields
        /// </summary>
        [TestMethod]
        public void fillEmptyEventFieldsTest()
        {
            FbCrawler crawler = new FbCrawler();
            Place place = new Place();
            Event fbEvent = new Event();

            place.name = "Bonnstr./ Ecke Aachenerstr. Haltestelle Weiden- West, 50226 Frechen";
            fbEvent.place = place;

            crawler.FillEmptyEventFields(fbEvent);

        }
    }
}
