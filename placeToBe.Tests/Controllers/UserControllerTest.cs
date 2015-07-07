using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe;
using placeToBe.Controllers;
using System.Threading.Tasks;
using System.Net;

namespace placeToBe.Tests.Controllers
{
    [TestClass]
    public class UserControllerTest
    {
        [TestMethod]
        public void PostServerError()
        {
            //Arrange 
            UserController controller = new UserController();
            String userEmail = "exampleplacetobe123@gmail.com";
            String userPassword = "example123";
            HttpStatusCode expected = HttpStatusCode.InternalServerError;

            //doesnt work with ServiceUnavailable

            //Act
            Task<HttpStatusCode> status = controller.Post(userEmail, userPassword);
            HttpStatusCode result = status.Result;

            //Assert
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void PostOk()
        {   //espects an email + password which arent registred yet
            //Arrange 
            UserController controller = new UserController();
            String userEmail = "";
            String userPassword = "";
            HttpStatusCode expected = HttpStatusCode.OK;

            //Act
            Task<HttpStatusCode> status = controller.Post(userEmail, userPassword);
            HttpStatusCode result = status.Result;

            //Assert
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void GetServerError()
        {
            //Arrange 
            UserController controller = new UserController();
            String activationcode = "aeh76";
            HttpStatusCode expected = HttpStatusCode.InternalServerError;

            //doesnt work with ServiceUnavailable

            //Act
            Task<HttpStatusCode> status = controller.Get(activationcode);
            HttpStatusCode result = status.Result;

            //Assert
            Assert.AreEqual(result, expected);
        }
        
    }
}
