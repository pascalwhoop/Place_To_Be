using placeToBe.Model;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace placeToBe.Services
{
    public class GenderizeService
    {

         MongoDbRepository<Gender> repo = new MongoDbRepository<Gender>();
         MongoDbRepository<Event> repoEvent = new MongoDbRepository<Event>();
        public String URL { get; set; }

        /// <summary>
        /// Search for a gender by name and returns it.
        /// </summary>
        /// <param name="name">name of a person</param>
        /// <returns>gender of the name</returns>
        public async Task<Gender> GetGender(String name)
        {
          Gender gender =  await repo.GetByIdAsync(name);
          return gender;
        }

        /// <summary>
        /// GetGender uses the genderize.io API to get the gender of a prename
        /// </summary>
        /// <param name="name">using a name of a person to get the gender</param>
        public void SetGender(String name)
        {
            String result;
            Gender gender;

            HttpWebRequest request;
            String getData = "name=" + name;
            URL = "http://api.genderize.io/?";
            Uri uri = new Uri(URL + getData);
            request = (HttpWebRequest)WebRequest.Create(uri);

            request.Method = "GET";

            request.AllowAutoRedirect = true;

            UTF8Encoding enc = new UTF8Encoding();

            HttpWebResponse Response;
            try
            {
                using (Response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = Response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            //String of the json from genderize.io
                            result = readStream.ReadToEnd();

                            //convert String to c# Object
                            gender = GenderToObject(result);

                            //push the Object to DB
                            PushGenderToDb(gender);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Get the amount of males and females for an event
        /// </summary>
        /// <param name="eventGenStat"></param>
        /// <returns>returns an object with param male and female, which contain the amount of each</returns>
        public async Task<GenderStat> GetGenderStat(Event eventGenStat)//event oder doch direkt ueber id?
        {
            int male=0;
            int female=0;

            String id= eventGenStat.fbId;
            Event Event = await repoEvent.GetByIdAsync(id);
            //Get list of people going to the event
            List<Rsvp> list = Event.attending;
            Rsvp[] array = list.ToArray();

            for (int i = 0; i < array.Length; i++)
            {
               Gender gender= await GetGender(array[i].name);
               if (gender.gender == "male")
               {
                   male++;
               }
               else
               {
                   female++;
               }
            }

            GenderStat genderStat = new GenderStat(male, female);
            return genderStat;
            
        }

        #region HelperMethods
        private Gender GenderToObject(String result)
        {
            String json = @result;
            Gender gender = new Gender(json);
            return gender;
        }

        private void PushGenderToDb(Gender gender)
        {
            repo.InsertAsync(gender);
        }

        #endregion
    }
}