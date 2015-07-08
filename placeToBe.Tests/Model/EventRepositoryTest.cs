using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe.Model.Repositories;
using placeToBe.Model.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace placeToBe.Tests.Model
{
    [TestClass]
    public class EventRepositoryTest
    {

        [TestMethod]
        public void InsertAnEventForTestsEventRepository()
        {
            //Arrange
            MongoDbRepository<Event> repo = new MongoDbRepository<Event>();
            Event eventtest = new Event();
            eventtest.startDateTime = new DateTime(2015, 5, 3, 20, 0, 0);
            eventtest.endDateTime = new DateTime(2015, 5, 3, 23, 0, 0);
            eventtest.name = "DummyEvent";
            eventtest.geoLocationCoordinates = new GeoLocation(50, 6);
            eventtest.attendingCount = 50;

            //Act
            repo.InsertAsync(eventtest);
        }

        [TestMethod]
        public void getEventsByTimeAndPolygon()
        {
            //Arrange
            EventRepository repo = new EventRepository();
            DateTime startTime = new DateTime(2015, 5, 3, 20, 0, 0);
            DateTime endTime = new DateTime(2015, 5, 3, 23, 0, 0);
            double[,] polygon = new double[50, 6];
            LightEvent testevent = new LightEvent();
            int count = testevent.attendingCount = 50;


            //Act
            Task<List<LightEvent>> task = repo.getEventsByTimeAndPolygon(polygon, startTime, endTime);
            List<LightEvent> eventi = task.Result;


            //Assert
            Assert.IsNotNull(eventi);
        }


    }
}
