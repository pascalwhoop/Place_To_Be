using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe;
using placeToBe.Model.Repositories;
using placeToBe.Model.Entities;
using placeToBe.Model;

namespace placeToBe.Tests.Model
{
    //author: Merve Bas 

    /// <summary>
    /// Class is testing methods of the class MongoDbRepository
    /// Procedure: 
    /// creating dummy- parameters and passing it over 
    /// MongoDbRepository contains CRUD Operations
    /// </summary>
    [TestClass]
    public class MongoDbRepositoryTest
    {
        MongoDbRepository<Event> model = new MongoDbRepository<Event>();

        /// <summary>
        /// Testing the method insertAsnyc of class MongoDbRepository for practicability 
        /// </summary>
        [TestMethod]
        public void insertAsync()
        {
            //Arrange

            Event test = new Event();
            test.name = "Hallo";
            test.privacy = "public";

            //Act
            model.InsertAsync(test);

            //Arrange
        }

        /// <summary>
        /// Testing the method updateAsnyc of class MongoDbRepository for practicability 
        /// </summary>
        [TestMethod]
        public void updateAsync()
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

        /// <summary>
        /// Testing the method deleteAsnyc of class MongoDbRepository for practicability 
        /// </summary>
        [TestMethod]
        public void deleteAsyncTest()
        {
            //Arrange
            Event test = new Event();

            //Act
            model.DeleteAsync(test);

            //Arrange
        }

        /// <summary>
        /// Testing the method searchForAsnyc of class MongoDbRepository for practicability 
        /// </summary>
        [TestMethod]
        public void searchForAsync()
        {
            //Arrange
            string filtertext = "filter";

            //Act
            model.SearchForAsync(filtertext);

            //Arrange
        }

        /// <summary>
        /// Testing the method getAllAsnyc of class MongoDbRepository for practicability 
        /// </summary>
        [TestMethod]
        public void getAllAsync()
        {
            //Arrange


            //Act
            model.GetAllAsync();

            //Arrange
        }

        

    }

}
