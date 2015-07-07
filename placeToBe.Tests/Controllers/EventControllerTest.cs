using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe;
using placeToBe.Controllers;
using placeToBe.Model.Entities;
using System.Threading.Tasks;

namespace placeToBe.Tests.Controllers
{
    [TestClass]
    public class EventControllerTest
    {
        [TestMethod]
        public void getEventsByTimeAndCity()
        {
            //Arrange
            EventController controller = new EventController();
            String id = "12312312301";
            String year = "2015";
            String month = "06";
            String day = "Monday";
            String hour = "17:00";

            //Act
            controller.getEventsByTimeAndCity(id, year, month, day, hour);
        }

        [TestMethod]
        public void getNearEventsByPointWithDescription()
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
            controller.getNearEventsByPointWithDescription(latitude, longitude, radius, year, month, day, hour);

        }

        [TestMethod]
        public void GetEvent()
        {
            //Arrange
            EventController controller = new EventController();
            Guid id = Guid.NewGuid();

            //Act
            controller.GetEvent(id);

         
        }
    }
}
