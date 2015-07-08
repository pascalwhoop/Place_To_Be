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
    [TestClass]
    public class UserControllerTest
    {
        [TestMethod]
        public void PostSaveFbData()
        {
            //Arrange 
            UserController controller = new UserController();
            User user= new User();

            //Act
            Task<User> status = controller.Post(user);

        }

        [TestMethod]
        public void PutPasswordChangePairBasRequest()
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

        [TestMethod]
        public void PutPasswordChangeNotFound()
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
