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
        MongoDbRepository<Event> model = new MongoDbRepository<Event>();


        [TestMethod]
        public void InsertAsync()
        {
            //Arrange

            Event test = new Event();
            test.name = "Hallo";
            test.privacy = "public";

            //Act
            model.InsertAsync(test);

            //Arrange
        }

        [TestMethod]
        public void UpdateAsync()
        {
            //Arrange
            Event test = new Event();
            test.name = "Hallo";
            test.privacy = "public";
            test.description = "Dies ist ein Testevent!";

            //Act
            model.UpdateAsync(test);

            //Arrange
        }

        [TestMethod]
        public void DeleteAsync()
        {
            //Arrange
            Event test = new Event();

            //Act
            model.DeleteAsync(test);

            //Arrange
        }

        [TestMethod]
        public void SearchForAsync()
        {
            //Arrange
            string filtertext = "filter";

            //Act
            model.SearchForAsync(filtertext);

            //Arrange
        }

        [TestMethod]
        public void GetAllAsync()
        {
            //Arrange


            //Act
            model.GetAllAsync();

            //Arrange
        }

        [TestMethod]
        public void GetByIdAsync()
        {
            //Arrange
            String name = "";


            //Act
            model.GetByIdAsync(name);

            //Arrange
        }

    }

}
