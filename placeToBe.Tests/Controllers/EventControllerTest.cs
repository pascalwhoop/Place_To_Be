using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe.Controllers;

namespace placeToBe.Tests.Controllers
{
    //author: Merve Bas 

    /// <summary>
    /// Class is testing methods of the class EventControllerService
    /// Procedure: 
    /// creating dummy- parameters and passing it over 
    /// </summary>
    [TestClass]
    public class EventControllerTest
    {
        /// <summary>
        /// Testing the method getEventsByTimeAndCity of class EventController for practicability
        /// </summary>
        [TestMethod]
        public void getEventsByTimeAndCityTest()
        {
            //Arrange
            EventController controller = new EventController();
            String id = "12312312301";
            String year = "2015";
            String month = "06";
            String day = "Monday";
            String hour = "17:00";

            //Act
            var task = controller.getEventsByTimeAndCity(id, year, month, day, hour);

            //Assert
            Assert.IsNotNull(task);
        }

        /// <summary>
        /// Testing the method getNearEventsByPointWithDescription of class EventController for practicability 
        /// </summary>
        [TestMethod]
        public void getNearEventsByPointWithDescriptionTest()
        {
            //Arrange
            EventController controller = new EventController();
            String latitude = "50.94";
            String longitude = "6.95";
            String radius = "50";
            String year = "2015";
            String month = "06";
            String day = "Monday";
            String hour = "17:00";

            //Act
            var task = controller.getNearEventsByPointWithDescription(latitude, longitude, radius, year, month, day, hour);

            //Assert
            Assert.IsNotNull(task);
        }

        /// <summary>
        /// Testing the method getEvent of class EventController for practicability 
        /// </summary>
        [TestMethod]
        public void getEventTest()
        {
            //Arrange
            EventController controller = new EventController();
            Guid id = Guid.NewGuid();

            //Act
            var task = controller.getEvent(id);

            //Assert 
            Assert.IsNotNull(task);


        }
    }
}
