using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe.Model.Repositories;
using System.Threading.Tasks;
using placeToBe.Model.Entities;
using System.Collections.Generic;

namespace placeToBe.Tests.Model
{
    [TestClass]
    public class CityRepositoryTest
    {
        [TestMethod]
        public void GetAllAsync()
        {
            //Arrange
            CityRepository repo = new CityRepository();

            //Act
            Task<IList<City>> task = repo.GetAllAsync();

            //Assert
            Assert.IsNotNull(task);


        }

        [TestMethod]
        public void InsertAPlaceForTestgteByPlaceId()
        {
            //Arrange
            MongoDbRepository<City> repo = new MongoDbRepository<City>();
            City city = new City();
            city.place_id = "252874629056";
            city.formatted_address = "cologne";

            //Act
            repo.InsertAsync(city);
        }

        [TestMethod]
        public void getByPlaceId()
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
