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

            //event test werte zuweisen?
            //wie zuweisen?
            Event test = new Event();

            //Act
           model.InsertAsync(test);

            //Arrange
        }

        [TestMethod]
        public void UpdateAsync()
        {
            //Arrange
            MongoDbRepository<Event> model = new MongoDbRepository<Event>();
            Event test = new Event();

            //Act
            model.UpdateAsync(test);

            //Arrange
        }

        [TestMethod]
        public void DeleteAsync()
        {
            //Arrange
            MongoDbRepository<Event> model = new MongoDbRepository<Event>();
            Event test = new Event();

            //Act
            model.DeleteAsync(test);

            //Arrange
        }
    }
}
