using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe.Controllers;

namespace placeToBe.Tests.Controllers
{
    [TestClass]
    public class CityControllerTest
    {
        [TestMethod]
        public void GetCityController()
        {
            //Arrange
            CityController controller = new CityController();

            //Act
            var task = controller.Get();

            //Assert
            Assert.IsNotNull(task);
        }
    }
}
