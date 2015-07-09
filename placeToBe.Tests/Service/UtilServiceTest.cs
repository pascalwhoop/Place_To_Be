using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe.Model.Entities;
using placeToBe.Services;

namespace placeToBe.Tests.Service
{
    //author: Merve Bas 

    /// <summary>
    /// Class is testing method of the class UtilService
    /// Procedure: 
    /// creating dummy- parameters and passing it over 
    /// </summary>

    [TestClass]
    public class UtilServiceTest
    {

        String dateString = "2014-12-20T19:00:00+0100";

        /// <summary>
        /// Testing the method getTimeFromISOString of class AccountService for practicability
        /// The test compares an expected result for a given String and the actual result 
        /// </summary>
        [TestMethod]
        public void getTimeFromISOStringTest()
        {
            //Arrange
            DateTime expected = new DateTime(2014, 12, 20, 19, 0, 0);

            //Act
            DateTime date = UtilService.getDateTimeFromISOString(dateString);

            //Assert 
            Assert.AreEqual(expected, date);

        }
    }
}