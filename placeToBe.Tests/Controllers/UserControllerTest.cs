using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe;
using placeToBe.Controllers;
using System.Threading.Tasks;
using System.Net;
using placeToBe.Model.Entities;
using System.Web.Mvc;

namespace placeToBe.Tests.Controllers
{
    //author: Merve Bas 

    /// <summary>
    /// Class is testing methods of the class UserController
    /// Especially these methods are goint to be tested also by BlackBox tests by 
    /// simulating a user passing correct and incorrect Inputs
    /// Procedure: 
    /// creating dummy- parameters and passing it over 
    /// </summary>
    [TestClass]
    public class UserControllerTest
    {
        /// <summary>
        /// Testing the method post of class UserController for practicability 
        /// </summary>
        [TestMethod]
        public void postCreateUserTest()
        {
            //Arrange 
            UserController controller = new UserController();
            User user= new User();
            user.city = "DummyCity";
            user.email = "DummyEmail";

            //Act
            Task<User> status = controller.Post(user);

        }
        /// <summary>
        /// Testing the method put of class UserController for practicability 
        /// with invalid input so the method returns a BadRequest Exception
        /// </summary>
        [TestMethod]
        public void putPasswordChangePairBadRequestTest()
        {
            //Arrange 
            UserController controller = new UserController();
            PasswordChangePair pair = new PasswordChangePair();
            pair.email = "placetobecologne";
            pair.oldPassword = "falsePassword";
            pair.newPassword = "something";

            //Act
            var task = controller.Put(pair);

            HttpStatusCodeResult result = (HttpStatusCodeResult)task.Result;

            //Assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Testing the method put of class UserController for practicability 
        /// with invalid input so the method returns a NotFound Exception
        /// </summary>
        [TestMethod]
        public void putPasswordChangeNotFound()
        {
            //Arrange 
            UserController controller = new UserController();
            PasswordChangePair pair = new PasswordChangePair();
            pair.email = "";
            pair.oldPassword = "falsePassword";
            pair.newPassword = "something";

            //Act
            var task = controller.Put(pair);

            HttpStatusCodeResult result = (HttpStatusCodeResult)task.Result;

            //Assert
            Assert.IsNotNull(result);
        }

    }
}
