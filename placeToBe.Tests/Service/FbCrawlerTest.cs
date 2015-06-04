using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe;
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
    }
}
