using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe;
using placeToBe.Model.Repositories;
using placeToBe.Model.Entities;
using placeToBe.Model;

namespace placeToBe.Tests.Model
{
    [TestClass]
    public class MongoDbRepositoryTest
    {
        [TestMethod]
        public void InsertAsync()
        {
            //Arrange
            MongoDbRepository <Event> model = new MongoDbRepository <Event> ();
            //Event test = ;

            //Act
            // model.InsertAsync(test);
        }
    }
}
