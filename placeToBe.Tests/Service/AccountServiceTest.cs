using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe.Services;
using placeToBe.Model.Repositories;

namespace placeToBe.Tests.Service
{
    [TestClass]
    public class AccountServiceTest
    {
        [TestMethod]
        public void Register()
        {
            //Arrange

            AccountService account = new AccountService();
            string email = "exampleplacetobe@hotmail.de";
            string password = "exampleplacetobe123";

            //Act

            account.Register(email,password);

            //Assert
            //Frage, ob hier eine Asser Anweisung zugehoert
           
        }

        [TestMethod]
        public void Login()
        {
           //Arrange

            AccountService account = new AccountService();
            string email = "exampleplacetobe@hotmail.de";
            string password = "exampleplacetobe123";

            //Act 

            account.Login(email, password);

            //Assert

            //Assert.AreEqual()
            

            /*
            
            //versuch mit nicht registrierter mail adresse oder passwort
            //Test funktioniert auch mit nicht registrierten Informationen 

            //Arrange

            AccountService account = new AccountService();
            string email = "exampleplacetobenicht@hotmail.de";
            string password = "exampleplacetobe123678";


            //Act 

            account.Login(email, password);
             */
        }


    }
}
