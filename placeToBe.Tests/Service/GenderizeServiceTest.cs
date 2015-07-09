using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe;
using placeToBe.Services;
using placeToBe.Model;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace placeToBe.Tests.Service
{
    //author: Merve Bas 

    /// <summary>
    /// Class is testing methods of the class GenderizeService
    /// Procedure: 
    /// creating dummy- parameters and passing it over 
    /// </summary>
    [TestClass]
    public class GenderizeServiceTest
    {
        /// <summary>
        /// Testing the method getGender of class GenderizeService 
        /// Procedure: 
        /// Passing a female name as dummy- parameter over and comparing the expected result
        /// "female" with the actual result of the methode
        /// </summary>
        [TestMethod]
        public void getGenderFemale()
        {

            //Arrange 
            GenderizeService service = new GenderizeService();


            //Test female 
            string namefemale = "Laura";
            string expectedfemale = "female";

            //Act
            Task<Gender> gendertaskfemale = service.getGender(namefemale);
            Gender genderfemale = gendertaskfemale.Result;
            String resultfemale = genderfemale.gender;



            //Assert
            Assert.AreEqual(resultfemale, expectedfemale);


        }

        /// <summary>
        /// Testing the method getGender of class GenderizeService 
        /// Procedure: 
        /// Passing a male name as dummy- parameter over and comparing the expected result
        /// "male" with the actual result of the methode
        /// </summary>
        [TestMethod]
        public void getGenderMale()
        {
            //Arrange
            GenderizeService service = new GenderizeService();
            //Test male 
            string namemale = "Johan";
            string expectedmale = "male";

            //Act
            Task<Gender> gendertaskmale = service.getGender(namemale);
            Gender gendermale = gendertaskmale.Result;
            String resultmale = gendermale.gender;


            //Assert
            Assert.AreEqual(resultmale, expectedmale);
        }

        /// <summary>
        /// Testing the method getGender of class GenderizeService 
        /// Procedure: 
        /// Passing an unisex name as dummy- parameter over and comparing the expected result
        /// Test doesnt work with unisex names
        /// Unisex names like kim are either getting the result "male" oder "female"
        /// </summary>
        [TestMethod]
        public void getGenderUni()
        {
            //Test doenst work with unisex names
            //Test expects for "Kim" the gender female

            //Arrange
            GenderizeService service = new GenderizeService();
            //Test unisex 
            string nameuni = "Kim";
            //string expecteduni1 = "male";   //testing both genders for 
            string expecteduni2 = "female"; //unisex names 

            //Act
            Task<Gender> gendertaskuni = service.getGender(nameuni);
            Gender genderuni = gendertaskuni.Result;
            String resultuni = genderuni.gender;


            //Assert
            //Assert.AreEqual(resultuni, expecteduni1);
            Assert.AreEqual(resultuni, expecteduni2);

        }

        /// <summary>
        /// Testing the method getGender of class GenderizeService 
        /// Procedure: 
        /// Passing a female name as dummy- parameter over and comparing the expected result
        /// "female" with the actual result of the methode
        /// </summary>
        [TestMethod]
        public void getGenderFromApiFemale()
        {
            //Arrange
            GenderizeService service = new GenderizeService();
            //Test female
            String namefemale = "Laura";
            String expectedfemale = "female";

            //Act
            Gender genderfemale = service.getGenderFromApi(namefemale);
            String resultfemale = genderfemale.gender;

            //Assert
            Assert.AreEqual(resultfemale, expectedfemale);

        }

        /// <summary>
        /// Testing the method getGender of class GenderizeService 
        /// Procedure: 
        /// Passing a male name as dummy- parameter over and comparing the expected result
        /// "male" with the actual result of the methode
        /// </summary>
        [TestMethod]
        public void getGenderFromApiMale()
        {
            //Arrange
            GenderizeService service = new GenderizeService();
            //Test male
            String namemale = "Johan";
            String expectedmale = "male";

            //Act
            Gender gendermale = service.getGenderFromApi(namemale);
            String resultmale = gendermale.gender;

            //Assert
            Assert.AreEqual(resultmale, expectedmale);

        }

        /// <summary>
        /// Testing the method getGender of class GenderizeService 
        /// Procedure: 
        /// Passing an unisex name as dummy- parameter over and comparing the expected result
        /// Test doesnt work with unisex names
        /// Unisex names like kim are either getting the result "male" oder "female"
        /// </summary>
        [TestMethod]
        public void getGenderFromApiUni()
        {
            //Test doenst work with unisex names
            //Test expects for "Kim" the gender female

            //Arrange
            GenderizeService service = new GenderizeService();
            //Test unisex
            String nameuni = "Kim";
            String expecteduni1 = "female"; //testing both genders
            //String expecteduni2 = "male";//for unisex names

            //Act
            Gender genderuni = service.getGenderFromApi(nameuni);
            String resultuni = genderuni.gender;

            //Assert
            Assert.AreEqual(resultuni, expecteduni1);
            //Assert.AreEqual(resultuni, expecteduni2);
        }

       
    }
}
