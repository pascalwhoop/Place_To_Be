﻿using MongoDB.Driver;
using placeToBe.Model;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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


        /// <summary>
        /// return the gender statistik of a specific event
        /// </summary>
        /// <param name="fbId">id of an event</param>
        /// <returns>return an int[] array with value of array[0]=male, array[1]=female, array[2]=undifined</returns>
        public async Task<int[]> GetGenderStat(String fbId)
        {
            int[] genderStat = new int[3];
            Event eventNew = await SearchDbForEvent(fbId);
            if (eventNew.attendingMale == null && eventNew.attendingFemale == null)
            {
                genderStat = await CreateGenderStat(eventNew);
            }
            else
            {
                genderStat[0] = eventNew.attendingMale;
                genderStat[1] = eventNew.attendingFemale;
                genderStat[2] = eventNew.attendingUndifined;
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
                gender = null;
            }

            if (gender == null)
            {
                gender = GetGenderFromApi(name);

                PushGenderToDb(gender);
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
            Gender gender;

            HttpWebRequest request;
            String getData = "name=" + name;
            URL = "http://api.genderize.io/?";
            Uri uri = new Uri(URL + getData);
            request = (HttpWebRequest)WebRequest.Create(uri);

            request.Method = "GET";

            request.AllowAutoRedirect = true;

            UTF8Encoding enc = new UTF8Encoding();
            if ( xRateLimitRemaining == 0)
            {
                if (xRateReset == 0)
                {
                    double difference = (DateTime.Now.AddDays(1) - DateTime.Now).TotalSeconds;
                    //round and seconds to milliseconds
                    int differenceInt = (Convert.ToInt32(Math.Floor(difference)))*1000;

                    Thread.Sleep(differenceInt);
                }
                else
                {
                    double difference = (DateTime.Now - this.lastRequest).TotalSeconds;
                    //round and seconds to milliseconds
                    int differenceInt = (Convert.ToInt32(Math.Floor(difference)))*1000;

                    int sleepDifference = xRateReset - differenceInt;
                    Thread.Sleep(sleepDifference);
                }
            }

            HttpWebResponse Response;
            try
            {
                using (Response = (HttpWebResponse)request.GetResponse())
                {

                    using (Stream responseStream = Response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            this.xRateLimitRemaining = Int32.Parse(Response.Headers["X-Rate-Limit-Remaining"]);
                            this.xRateReset = Int32.Parse(Response.Headers["X-Rate-Reset"]);
                            this.lastRequest = DateTime.Now;

                            //String of the json from genderize.io
                            result = readStream.ReadToEnd();

                            //convert String to c# Object
                            gender = GenderToObject(result);

                            return gender;
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
        /// <returns>returns array with array[0]=male, array[1]=female, array[2]=undifined</returns>
        public async Task<int[]> CreateGenderStat(Event eventNew)
        {
            int male = 0;
            int female = 0;
            int undifined = 0;

            Gender gender;


            //GET list of people attending the event
            List<Rsvp> list = eventNew.attending;
            Rsvp[] array = list.ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                gender = await GetGender(array[i].name);
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
                    undifined++;
                }
            }

            eventNew.attendingMale = male;
            eventNew.attendingFemale = female;
            eventNew.attendingUndifined = undifined;
            UpdateGenderStat(eventNew);

            int[] maleFemaleUndifined = { male, female, undifined };

            return maleFemaleUndifined;

        }

        #region HelperMethods
        private Gender GenderToObject(String result)
        {
            String json = @result;
            Gender gender = new Gender(json);
            return gender;
        }

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
                Thread.Sleep(15000);
                PushGenderToDb(gender);
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