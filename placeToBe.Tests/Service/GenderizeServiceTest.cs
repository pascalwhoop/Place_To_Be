using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe;
using placeToBe.Services;
using placeToBe.Model;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using System.Threading.Tasks;


namespace placeToBe.Tests.Service
{

    [TestClass]
    public class GenderizeServiceTest
    {
        [TestMethod]
        public void GetGenderFemale()
        {

            //Arrange 
            GenderizeService service = new GenderizeService();


            //Test female 
            string namefemale = "Laura";
            string expectedfemale = "female";

            //Act
            Task<Gender> gendertaskfemale = service.GetGender(namefemale);
            Gender genderfemale = gendertaskfemale.Result;
            String resultfemale = genderfemale.gender;



            //Assert
            Assert.AreEqual(resultfemale, expectedfemale);


        }

        [TestMethod]
        public void GetGenderMale()
        {
            //Arrange
            GenderizeService service = new GenderizeService();
            //Test male 
            string namemale = "Johan";
            string expectedmale = "male";

            //Act
            Task<Gender> gendertaskmale = service.GetGender(namemale);
            Gender gendermale = gendertaskmale.Result;
            String resultmale = gendermale.gender;


            //Assert
            Assert.AreEqual(resultmale, expectedmale);
        }

         [TestMethod]
        public void GetGenderUni()
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
            Task<Gender> gendertaskuni = service.GetGender(nameuni);
            Gender genderuni = gendertaskuni.Result;
            String resultuni = genderuni.gender;


            //Assert
            //Assert.AreEqual(resultuni, expecteduni1);
            Assert.AreEqual(resultuni, expecteduni2);

        }

        [TestMethod]
        public void GetGenderFromApiFemale()
        {
            //Arrange
            GenderizeService service = new GenderizeService();
            //Test female
            String namefemale = "Laura";
            String expectedfemale = "female";

            //Act
            Gender genderfemale = service.GetGenderFromApi(namefemale);
            String resultfemale = genderfemale.gender;

            //Assert
            Assert.AreEqual(resultfemale, expectedfemale);

        }

        [TestMethod]
        public void GetGenderFromApiMale()
        {
            //Arrange
            GenderizeService service = new GenderizeService();
            //Test male
            String namemale = "Johan";
            String expectedmale = "male";

            //Act
            Gender gendermale = service.GetGenderFromApi(namemale);
            String resultmale = gendermale.gender;

            //Assert
            Assert.AreEqual(resultmale, expectedmale);

        }

        [TestMethod]
        public void GetGenderFromApiUni()
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
            Gender genderuni = service.GetGenderFromApi(nameuni);
            String resultuni = genderuni.gender;

            //Assert
            Assert.AreEqual(resultuni, expecteduni1);
            //Assert.AreEqual(resultuni, expecteduni2);
        }

        [TestMethod]
        public void GetNamesArray()
        {
            //Arrange
            GenderizeService service = new GenderizeService();
            //fragen wie das aussehen soll 
            String[] ja;


        }

        [TestMethod]
        public void GetGenderStat()
        {
            //TODO Test funktioniert nicht, da auf die InsertAsync Methode nicht gewartet wird
            //Arrange 
            GenderizeService service = new GenderizeService();
            Event test = new Event();
            test.fbId = "123877171";
            test.attendingFemale = 150;
            test.attendingMale = 140;

            MongoDbRepository<Event> model = new MongoDbRepository<Event>();
            var task = model.InsertAsync(test);


            //Act
            Task<int[]> stat = service.CreateGenderStat(test);
            int[] genderstat = stat.Result;

            //Fragen was result ist, wie das aussieht 

        }
    }
}
