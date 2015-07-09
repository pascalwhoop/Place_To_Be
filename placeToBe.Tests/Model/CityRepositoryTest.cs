using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe.Model.Repositories;
using System.Threading.Tasks;
using placeToBe.Model.Entities;
using System.Collections.Generic;

namespace placeToBe.Tests.Model
{
    //author: Merve Bas 

    /// <summary>
    /// Class is testing methods of the class CityRepository
    /// Procedure: 
    /// creating dummy- parameters and passing it over 
    /// </summary>
    [TestClass]
    public class CityRepositoryTest
    {
        /// <summary>
        /// Testing the method getAllAsync of class CityRepository for practicability 
        /// </summary>
        [TestMethod]
        public void getAllAsyncTest()
        {
            //Arrange
            CityRepository repo = new CityRepository();

            //Act
            Task<IList<City>> task = repo.GetAllAsync();

            //Assert
            Assert.IsNotNull(task);


        }
        /// <summary>
        /// Method inserts a city so method getByPlaceId can be used with a non- filled database
        /// </summary>
        [TestMethod]
        public void insertAPlaceForTestGetByPlaceId()
        {
            //Arrange
            MongoDbRepository<City> repo = new MongoDbRepository<City>();
            City city = new City();
            city.place_id = "252874629056";
            city.formatted_address = "cologne";

            //Act
            repo.InsertAsync(city);
        }

        /// <summary>
        /// Testing the method getByPlaceID of class CityRepository for practicability 
        /// The expected adress is compared to the actual result of the method
        /// </summary>
        [TestMethod]
        public void getByPlaceIdTest()
        {
            //Arrange
            CityRepository repo = new CityRepository();
            String place_id = "252874629056";
            City city = new City();
            city.formatted_address = "cologne";


            //Act 
            Task<City> task = repo.getByPlaceId(place_id);
            City result = task.Result;

            //
            Assert.AreEqual(result.formatted_address, city.formatted_address);

        }
    }
}
