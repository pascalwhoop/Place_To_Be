using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe.Services;
using placeToBe.Model.Repositories;
using placeToBe.Model.Entities;
using System.Web;

namespace placeToBe.Tests.Service
{
    //author: Merve Bas 

    /// <summary>
    /// Class is testing methods of the class SearchService
    /// Procedure: 
    /// creating dummy- parameters and passing it over 
    /// </summary>
    [TestClass]
    public class SearchServiceTest
    {

        /// <summary>
        /// Testing the method heatSearch of class AccountService for practicability 
        /// </summary>
        [TestMethod]
        public void heatSearchTest()
        {
            //Arrange
            SearchService search = new SearchService();
            String id = "1234133a";
            DateTime startime = new DateTime(2015, 5, 3, 20, 0, 0);
            DateTime endtime = new DateTime(2015, 5, 3, 23, 0, 0);

            //Act
            var task = search.HeatSearch(id, startime, endtime);

            //Assert 
            Assert.IsNotNull(task);
        }

        /// <summary>
        /// Testing the method textSearch of class AccountService for practicability 
        /// </summary>
        [TestMethod]
        public void textSearchTest()
        {
            //Arrange
            SearchService search = new SearchService();
            String filter = "HipHop";

            //Act
            var task = search.TextSearch(filter);

            //Assert 
            Assert.IsNotNull(task);
        }

        /// <summary>
        /// Testing the method eventSearch of class AccountService for practicability 
        /// </summary>
        [TestMethod]
        public void eventSearchTest()
        {
            //Arrange
            SearchService search = new SearchService();
            Guid id = Guid.NewGuid();

            //Act
            var task = search.getEventByIdAsync(id);

            //Assert 
            Assert.IsNotNull(task);
        }

      
    }


}
