using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe.Services;
using placeToBe.Model.Repositories;
using placeToBe.Model.Entities;
using System.Threading.Tasks;
using System.Net;

namespace placeToBe.Tests.Service
{   //author: Merve Bas 

    /// <summary>
    /// Class is testing methods of the class AccountService
    /// Especially these methods are goint to be tested also by BlackBox tests by 
    /// simulating a user passing correct and incorrect Inputs
    /// Procedure: 
    /// creating dummy- parameters and passing it over 
    /// </summary>

    [TestClass]
    public class AccountServiceTest
    {

        /// <summary>
        /// Testing the method createUserTest of class AccountService for practicability 
        /// </summary>
        [TestMethod]
        public void createUserTest()
        {
            //Arrange
            AccountService account = new AccountService();
            UserRepository userrepo = new UserRepository();

            User usertest = new User();
            usertest.city = "DummyCity";
            usertest.email = "DummyEmail";

            //Act
            var task = account.createUser(usertest);


            //Assert
            Assert.IsNotNull(task);

        }


        /// <summary>
        /// Testing the method sendActivationEmail of class AccountService
        /// </summary>
        [TestMethod]
        public void sendActivationEmailTest()
        {   //Arrange
            AccountService account = new AccountService();

            String userEmail = "placetobecologne@gmail.com";
            String userPassword = "placetobecologne";


            //Act
            Task<String> status = account.SendActivationEmail(userEmail, userPassword);
            String result = status.Result;

            //Assert
            Assert.IsNotNull(result);

        }



            //[TestMethod]
            //public void ConfirmEmailOk()
            //{
            //    //Arrange 
            //    AccountService account = new AccountService();
            //    String activationcode = "8cde706f-ca79-4a66-9fa9-f04a86b4129a";
            //    bool expected = true;

            //    //Act
            //    Task<bool> status = account.ConfirmEmail(activationcode);
            //    bool result = status.Result;


            //    //Assert
            //    Assert.AreEqual(result, expected);
            //}

            //[TestMethod]
            //public void ConfirmEmailServerError()
            //{
            //    //Arrange 
            //    AccountService account = new AccountService();
            //    String activationcode = "2187316c-71a3-4118-";
            //    HttpStatusCode expected = HttpStatusCode.InternalServerError;

            //    //Act
            //    Task<HttpStatusCode> status = account.ConfirmEmail(activationcode);
            //    HttpStatusCode result = status.Result;

            //    //Assert
            //    Assert.AreEqual(result, expected);
            //}


        /// <summary>
        /// Testing the method changePassword of class AccountService
        /// with invalid input so the method returns a NotFound Exception
        /// </summary>
        [TestMethod]
        public void changePasswordNotFoundTest()
        {
            //Arrange
            AccountService account = new AccountService();
            String userEmail = "";
            String oldPassword = "something";
            String newPassword = "something";
            HttpStatusCode expected = HttpStatusCode.NotFound;

            //Act
            Task<HttpStatusCode> status = account.ChangePasswort(userEmail, oldPassword, newPassword);
            HttpStatusCode result = status.Result;

            //Assert
            Assert.AreEqual(result, expected);

        }

        /// <summary>
        /// Testing the method changePassword of class AccountService
        /// with invalid input so the method returns a BasRequest Exception
        /// </summary>
        [TestMethod]
        public void changePasswordBasRequestTest()
        {
            //Arrange
            AccountService account = new AccountService();
            String userEmail = "placetobecologne@gmail.com";
            String oldPassword = "falsepassword";
            String newPassword = "something";
            HttpStatusCode expected = HttpStatusCode.BadRequest;

            //Act
            Task<HttpStatusCode> status = account.ChangePasswort(userEmail, oldPassword, newPassword);
            HttpStatusCode result = status.Result;

            //Assert
            Assert.AreEqual(result, expected);

        }

        /// <summary>
        /// Testing the method saveFbData of class AccountService for practicatily 
        /// </summary>
        [TestMethod]
        public void saveFbDataTest()
        {
            //Arrange
            AccountService account = new AccountService();
            FbUser usr = new FbUser();
            usr.firstName = "Laura";

            //Act
            Task<FbUser> task = account.SaveFBData(usr);

            //Assert
            Assert.IsNotNull(task);

        }

    }
}