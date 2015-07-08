using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe.Services;
using placeToBe.Model.Repositories;
using placeToBe.Model.Entities;
using System.Threading.Tasks;
using System.Net;

namespace placeToBe.Tests.Service
{
    [TestClass]
    public class AccountServiceTest
    {

        //Methods are goint to be tested also by BlackBox tests
        //to check for correct and incorrect inputs
        [TestMethod]
        public void SaveFbData()
        {
            //Arrange
            AccountService account = new AccountService();
            UserRepository userrepo = new UserRepository();

            FbUser usertest = new FbUser();
            usertest.firstName = "Merve";
            usertest.lastName = "Nur";
            usertest.emailFB = "examplemailfbmerve@googlemail.com";

            //Act
            var task = account.SaveFBData(usertest);


            //Assert
            Assert.IsNotNull(task);



        }



        [TestMethod]
        public void SendActivationEmailInternalServerError()
        {   //Arrange
            AccountService account = new AccountService();

            String userEmail = "placetobecologne@gmail.com";
            String userPassword = "placetobecologne";


            //Act
            Task<String> status = account.SendActivationEmail(userEmail, userPassword);
            String result = status.Result;

            //Assert
            //result ist the activationcode for the user 
            //the activationcode is changin everytime this methode is used
            Assert.IsNotNull(result);

        }




        /*

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

            /*
            [TestMethod]
            public void Login()
            {
               //Arrange

                AccountService account = new AccountService();
                String usersEmail = "exampleplacetobe@hotmail.de";
                String userpassword = "exampleplacetobe123";

                //Act 

                account.Login(usersEmail, userpassword);

                //Assert

           
            

                /*
            
                //versuch mit nicht registrierter mail adresse oder passwort
                //Test funktioniert auch mit nicht registrierten Informationen 
                //wird wahrscheinlich abgefangen durch try catch 

                //Arrange

                AccountService account = new AccountService();
                string email = "exampleplacetobenicht@hotmail.de";
                string password = "exampleplacetobe123678";


                //Act 

                account.Login(email, password);
             
            }
        */

        [TestMethod]
        public void ChangePasswordNotFound()
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

        [TestMethod]
        public void ChangePasswordBasRequest()
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
        [TestMethod]
        public void createUser()
        {
            //Arrange
            AccountService account = new AccountService();
            User usr = new User();
            usr.status = true;
            usr.city = "cologne";

            //Act
            Task<User> task = account.createUser(usr);

        }

    }
}