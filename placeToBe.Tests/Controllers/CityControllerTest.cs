using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe.Controllers;

namespace placeToBe.Tests.Controllers
{
    //author: Merve Bas 

    /// <summary>
    /// Class is testing method of the class CityController
    /// </summary>
    [TestClass]
    public class CityControllerTest
    {
        [TestMethod]
        public void getCityControllerTest()
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
