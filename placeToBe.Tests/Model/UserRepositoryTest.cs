using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe.Model.Repositories;
using placeToBe.Model.Entities;
using System.Threading.Tasks;
using placeToBe.Model.Repositories;
using System.Threading.Tasks;
using placeToBe.Model.Entities;
using System.Collections.Generic;

namespace placeToBe.Tests.Model
{
    [TestClass]
    public class UserRepositoryTest
    {
        [TestMethod]
        public void InsertsADummyUserForTest()
        {
            //Arrange
            MongoDbRepository<User> repo = new MongoDbRepository<User>();
            User usr = new User();
            usr.email = "dummyemailfortest@gmail.com";
            usr.activationcode = "9182716271dummy";
            usr.city = "Dummycity";

            //Act
            repo.InsertAsync(usr);

        }
        /*
        [TestMethod]
        public void GetByEmailAsync()
        {
            //Arrange
            UserRepository repo = new UserRepository();
            string email = "dummyemailfortest@gmail.com";
            User user = new User();
            user.city = "Dummycity";

            //Act
            User task = repo.GetByEmailAsync(email);

            //Assert
            Assert.IsNotNull(task);
        }
        */
        [TestMethod]
        public void getByActivationCodeTest()
        {
            //Arrange
            UserRepository repo = new UserRepository();
            String activationcode = "9182716271dummy";
            User user = new User();
            user.city = "Dummycity";

            //Act
            Task<User> task = repo.GetByActivationCode(activationcode);

            //Assert
            Assert.IsNotNull(task);

        }
    }
}
