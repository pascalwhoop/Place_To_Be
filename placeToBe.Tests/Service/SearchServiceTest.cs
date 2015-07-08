using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe.Services;
using placeToBe.Model.Repositories;
using placeToBe.Model.Entities;

namespace placeToBe.Tests.Service
{
    [TestClass]
    public class SearchServiceTest
    {


        [TestMethod]
        public void HeatSearch()
        {
            //Arrange
            SearchService search = new SearchService();
            String id = "1234133a";
            DateTime startime = new DateTime(2015, 5, 3, 20, 0, 0);
            DateTime endtime = new DateTime(2015, 5, 3, 23, 0, 0);

            //Act
            search.HeatSearch(id, startime, endtime);
        }

        [TestMethod]
        public void TextSearch()
        {
            //Arrange
            SearchService search = new SearchService();
            String filter = "HipHop";

            //Act
            search.TextSearch(filter);
        }

        [TestMethod]
        public void EventSearch()
        {
            //Arrange
            SearchService search = new SearchService();
            Guid id = Guid.NewGuid();

            //Act
            search.EventSearch(id);
        }

        [TestMethod]
        public void findNearEventFromAPoint()
        {
            //Arrange
            SearchService search = new SearchService();
            double latitude = 19;
            double longitude = 23;
            int radius = 50;
            DateTime startime = new DateTime(2015, 5, 3, 20, 0, 0);
            DateTime endtime = new DateTime(2015, 5, 3, 23, 0, 0);

            search.findNearEventFromAPoint(latitude, longitude, radius, startime, endtime);


        }
    }


}
