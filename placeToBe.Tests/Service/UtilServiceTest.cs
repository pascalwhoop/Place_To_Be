using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe.Model.Entities;
using placeToBe.Services;

namespace placeToBe.Tests.Service {

    [TestClass]
    public class UtilServiceTest {

        String dateString = "2014-12-20T19:00:00+0100";


        [TestMethod]
        public void getTimeFromISOStringTest() {
            UtilService.getDateTimeFromISOString(dateString);
        }

        [TestMethod]
        public void eventDateSetterTest() {
            var e = new Event();
            e.start_time = dateString;
        }
    }
}