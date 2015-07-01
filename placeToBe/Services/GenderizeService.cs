using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using Facebook;
using Microsoft.Ajax.Utilities;
using MongoDB.Driver;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace placeToBe.Services
{
    public class GenderizeService
    {

        GenderRepository repoGender = new GenderRepository();
        EventRepository repoEvent = new EventRepository();
        public String URL { get; set; }
        public int xRateLimitRemaining;
        public int xRateReset;
        public DateTime lastRequest;


        public List<String> GetPrenamesStringArray(List<Rsvp> rsvpArray)
        {
            List<String> onlyPrenameList = new List<String>();
            String[] splitItem;
            
            foreach (var item in rsvpArray)
            {
                splitItem=item.name.Split(new[] {" ", "-"}, StringSplitOptions.None);
                onlyPrenameList.Add(splitItem[0]);

            }
            return onlyPrenameList;
        }


        /// <summary>
        /// return the gender statistik of a specific event
        /// </summary>
        /// <param name="fbId">id of an event</param>
        /// <returns>return an int[] array with value of array[0]=male, array[1]=female, array[2]=undifined</returns>
        public async Task<int[]> GetGenderStat(String fbId)
        {
            int[] genderStat = new int[3];
            Event eventNew = await SearchDbForEvent(fbId);
            if (eventNew.attendingMale == 0 && eventNew.attendingFemale == 0 && eventNew.attendingUndefined == 0)
            {
                genderStat = await CreateGenderStat(eventNew);
            }
            else
            {
                genderStat[0] = eventNew.attendingMale;
                genderStat[1] = eventNew.attendingFemale;
                genderStat[2] = eventNew.attendingUndefined;
            }

            return genderStat;
        }

        /// <summary>
        /// Search for a gender by name and returns it.
        /// </summary>
        /// <param name="name">name of a person</param>
        /// <returns>gender of the name</returns>
        public async Task<Gender> GetGender(String name)
        {
            Gender gender;
            try
            {
                gender = await SearchDbForGender(name);
                
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                gender = null;
            }

            if (gender == null)
            {
                gender = GetGenderFromApi(name);
                Debug.WriteLine("######## Got it from Api");

                PushGenderToDb(gender);
            }
            else
            {
                Debug.WriteLine("######### Already in DB");
            }

            return gender;
        }

        /// <summary>
        /// GetGender uses the genderize.io API to get the gender of a prename
        /// </summary>
        /// <param name="name">using a name of a person to get the gender</param>
        public Gender GetGenderFromApi(String name)
        {
            String result;
            Gender gender = null;

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
                            xRateLimitRemaining = Int32.Parse(Response.Headers["X-Rate-Limit-Remaining"]);
                            xRateReset = Int32.Parse(Response.Headers["X-Rate-Reset"]);
                            lastRequest = DateTime.Now;

                            //String of the json from genderize.io
                            result = readStream.ReadToEnd();
                            gender = JsonConvert.DeserializeObject<Gender>(result);
                            if (gender.gender == null)
                            {
                                gender.gender = "undifined";
                            }

                            return gender;
                        }
                    }
                }
            }
            catch (WebException webEx)
            {
                if (xRateLimitRemaining == 0)
                {
                    if (xRateReset == 0)
                    {
                        double difference = (DateTime.Now.AddDays(1) - DateTime.Now).TotalSeconds;
                        //round and seconds to milliseconds
                        int differenceInt = (Convert.ToInt32(Math.Floor(difference))) * 1000;
                        Debug.WriteLine("####### Waiting "+ differenceInt);
                        Thread.Sleep(differenceInt);
                    }
                    else
                    {
                        double difference = (DateTime.Now - lastRequest).TotalSeconds;
                        //round and seconds to milliseconds
                        int differenceInt = (Convert.ToInt32(Math.Floor(difference)));

                        int sleepDifference = (xRateReset - differenceInt) * 1000;
                        Debug.WriteLine("####### Waiting " + sleepDifference);
                        Thread.Sleep(sleepDifference);
                    }
                    return GetGenderFromApi(name);
                }
                Debug.WriteLine("Error: " + webEx.Message);
                throw webEx;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                if (gender != null)
                {
                    gender.name = name;
                    gender.gender = "undifined";
                    gender.count = 0;
                    gender.probability = 0;
                    return gender;
                }
                else
                {
                    throw;
                }
            }

        }

        /// <summary>
        /// Get the amount of males and females for an event
        /// </summary>
        /// <param name="eventGenStat"></param>
        /// <returns>returns array with array[0]=male, array[1]=female, array[2]=undifined</returns>
        public async Task<int[]> CreateGenderStat(Event eventNew)
        {
            int male = 0;
            int female = 0;
            int undefined = 0;

            Gender gender;


            //GET list of people attending the event
            List<Rsvp> attendingList = eventNew.attending;
            List<String> preNameList=GetPrenamesStringArray(attendingList);

            foreach (string name in preNameList)
            {
                gender = await GetGender(name);
                if (gender.gender == "male")
                {
                    male++;
                }
                else if (gender.gender == "female")
                {
                    female++;
                }
                else
                {
                    undefined++;
                }
            }

            eventNew.attendingMale = male;
            eventNew.attendingFemale = female;
            eventNew.attendingUndefined = undefined;
            UpdateGenderStat(eventNew);

            int[] maleFemaleUndifinedArray = { male, female, undefined };

            return maleFemaleUndifinedArray;

        }

        #region HelperMethods
        //private Gender GenderToObject(String result)
        //{
        //    String json = @result;
        //    Gender gender = new Gender(json);
        //    return gender;
        //}

        private async void UpdateGenderStat(Event eventNew)
        {
            try
            {
                await repoEvent.UpdateAsync(eventNew);
            }
            catch (MongoWriteException e)
            {
                Console.Write(e.Message);
            }
            catch (MongoWaitQueueFullException ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                Thread.Sleep(15000);
                UpdateGenderStat(eventNew);
            }
        }

        private async void PushGenderToDb(Gender gender)
        {
            try
            {
                await repoGender.InsertAsync(gender);
            }
            catch (MongoWriteException e)
            {
                Console.Write(e.Message);
            }
            catch (MongoWaitQueueFullException ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                Thread.Sleep(15000);
                PushGenderToDb(gender);
            }
            catch (AggregateException)
            {
                
                Debug.WriteLine("Invalid -> dont save");
            }
        }

        private async Task<Event> SearchDbForEvent(String fbId)
        {
            return await repoEvent.GetByFbIdAsync(fbId);
        }

        public async Task<Gender> SearchDbForGender(String name)
        {
            return await repoGender.GetByNameAsync(name);
        }

        #endregion
    }
}