using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe.Services;
using placeToBe.Model.Repositories;

namespace placeToBe.Tests.Service
{
    [TestClass]
    public class AccountServiceTest
    {
        //Tests just check whether methods are working, or not
        //There are no Asserts (yet)
        //Methods are goint to be tested also by BlackBox tests
        //to check for correct and incorrect inputs

        [TestMethod]
        public void SendActivationEmail()
        {   //Arrange
            AccountService account = new AccountService();
            String userEmail = "exampleplacetobe@hotmail.de";
            String userPassword = "exampleplacetobe123";

            //Act
            account.SendActivationEmail(userEmail, userPassword);

        }


        [TestMethod]
        public void Register()
        {
            //Arrange

            AccountService account = new AccountService();
            String userEmail = "exampleplacetobe@hotmail.de";
            String userPassword = "exampleplacetobe123";
            String activationcode = "12a3abcv";


            //Act

            account.Register(userEmail, userPassword, activationcode);

            //Assert
            //Frage, ob hier eine Asser Anweisung zugehoert
           
        }

        [TestMethod]
        public void ConfirmEmail()
        {
            //Arrange 
            AccountService account = new AccountService();
            String activationcode = "12a3abcv";

            //Act
            account.ConfirmEmail(activationcode);
        }

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
             */
        }

        [TestMethod]
        public void ChangePassword()
        {
            //Arrange
            AccountService account = new AccountService();
            String userEmail = "exampleplacetobe@hotmail.de";
            String oldPassword = "exampleplacetobe123";
            String newPassword = "exampleplacetobe123new";

            //Act
            account.ChangePasswort(userEmail, oldPassword, newPassword); 

        }

        [TestMethod]
        public void ForgetResetPassword()
        {
            //Arrange
            AccountService account = new AccountService();
            String userEmail = "exampleplacetobe@hotmail.de";

            //Act
            account.ForgetPasswordReset(userEmail);

            //SendForgetPassword is called by ForgetResetPassword

        }


    }
}
