using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe;
using placeToBe.Controllers;

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
    }
}
