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
            Task<Gender> gendertaskfemale = service.getGender(namefemale);
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
            Task<Gender> gendertaskmale = service.getGender(namemale);
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
            Task<Gender> gendertaskuni = service.getGender(nameuni);
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
            Gender genderfemale = service.getGenderFromApi(namefemale);
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
            Gender gendermale = service.getGenderFromApi(namemale);
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
            Gender genderuni = service.getGenderFromApi(nameuni);
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
            //TODO fixen
            /*
            //Arrange 
            GenderizeService service = new GenderizeService();
            Event test = new Event();
            Rsvp n = new Rsvp();
            List<Rsvp> rsvp= new List<Rsvp>();

            rsvp.Add(new Rsvp() { name= "Laura" }); ;
           /*test.attending.Add(new Rsvp() {i=> i.name = "Max", id = "a0123232", rsvp_status = "true" });
            test.attending.Add(new Rsvp() { name = "Jasmin", id = "a0123233", rsvp_status = "true" });
            test.attending.Add(new Rsvp() { name = "Thomas", id = "a123234", rsvp_status = "true" });
            test.attending.Add(new Rsvp() { name = "Kim", id = "a123235", rsvp_status = "true" });
            test.attending.Add(new Rsvp() { name = "Dominik", id = "a123236", rsvp_status = "true" });
            test.attending.Add(new Rsvp() { name = "Lisa", id = "a123237", rsvp_status = "true" });

            MongoDbRepository<Event> model = new MongoDbRepository<Event>();
            var task = model.InsertAsync(test);


            //Act
            Task<Event> stat = service.createGenderStat(test);
            Event genderstat = stat.Result;
            int attendingfemale = genderstat.attendingFemale;
            int attendingmale = genderstat.attendingMale;
            int attendingUndefined = genderstat.attendingUndefined;
            int attendingcount = genderstat.attendingCount;*/


            //Assert
            /*Assert.AreEqual(test.attendingFemale, attendingfemale);
            Assert.AreEqual(test.attendingMale, attendingmale);
            Assert.AreEqual(test.attendingUndefined, attendingUndefined);
             */




        }
    }
}
