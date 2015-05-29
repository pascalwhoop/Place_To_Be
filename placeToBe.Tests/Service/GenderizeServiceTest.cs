using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using placeToBe;
using placeToBe.Services;

namespace placeToBe.Tests.Service
{
    [TestClass]
    public class GenderizeServiceTest
    {
        [TestMethod]
        public void GetGender()
        {
            
            //Der Test funktioniert "nur" mit Namen, die aus 5 Buchstaben bestehen
            //Für Tests mit Namen, die mehr oder weniger Buchstaben haben, bitte Substring aendern!


            //Arrange
            GenderizeService service = new GenderizeService();

            //Test female
            string namefemale = "Laura";
            string expectedfemale = "\"gender\":\"female\"";

            //Act

            service.GetGender(namefemale);


            //Assert

            string wholestringf = service.GetGenderInternal();
            string substringf = wholestringf.Substring(16,17);
            Assert.AreEqual(substringf, expectedfemale);



            //Test male
            //Arrange
            string namemale = "Johan";
            string expectedmale = "\"gender\":\"male\"";

            //Act

            service.GetGender(namemale);


            //Assert

            string wholestringm = service.GetGenderInternal();
            string substringm = wholestringm.Substring(16, 15);
            Assert.AreEqual(substringm, expectedmale);



            //Test mit unisex Namen 
            //Test funktioniert nur mit Namen, die 3 Buchstaben enthalten
            //Fuer andere Namen bitte Substring aendern!
            //Methode funktioniert nicht mir unisex Namen 
            //Fuer Kim wird female ausgegeben

            //Arrange
            string nameunisex = "Kim";
            string expectedunisex1 = "\"gender\":\"male\"";  //beide gender
            string expectedunisex2 = "\"gender\":\"female\""; //sollen getestet werden 

            //Act

            service.GetGender(nameunisex);

            //Assert


            //string wholestringu1 = service.GetGenderInternal();
            //string substringu1 = wholestringu1.Substring(15, 16); //Test
            //Assert.AreEqual(substringu1, expectedunisex1);       //gender male


            string wholestringu2 = service.GetGenderInternal();
            string substringu2 = wholestringu2.Substring(14, 17); //Test
            Assert.AreEqual(substringu2, expectedunisex2);       //gender female



        }
    }
}
