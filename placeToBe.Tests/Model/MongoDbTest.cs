using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using placeToBe.Services;

namespace placeToBe.Tests.Model
{
    [TestClass]
    public class MongoDbTest
    {

        MongoDbRepository<User> userRepo = new MongoDbRepository<User>();
        AccountService accountService = new AccountService();
        
        [TestMethod]
        public void testUserInsertFetchAndFetchAgainBug()
        {
            User usr = new User() {
                activationcode = "4eb122ac-0ea5-467a-b016-878dda9b6584",
                city = "Cologne",
                company = null,
                email = "user@mail.de",

            };
        }
    }
}
