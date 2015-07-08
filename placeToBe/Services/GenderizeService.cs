using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Newtonsoft.Json;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;

namespace placeToBe.Services
{
    /// <summary>
    /// This class represents the genderizeservice, this service checks the name of a person and do a request to the
    /// genderize.io API to get its gender. With these genders the genderizeservice creates a statistik for a given event.
    /// </summary>
    public class GenderizeService
    {
        GenderRepository repoGender = new GenderRepository();
        // Represents the last request to the genderize.io
        public DateTime lastRequest;
        // Represents the amount of requests until genderize.io blocks our requests
        public int xRateLimitRemaining;
        // Represents the time after we got blocked, until we can do the next request
        public int xRateReset;
        //represents the next point in time when we can try again
        public DateTime xRateNextTry;
        public string url { get; set; }

        /// <summary>
        /// This method extract the prenames of the persons who are attending at an event
        /// </summary>
        /// <param name="rsvpArray">this array contains all person who are attending at an event</param>
        /// <returns>Returns a list of strings, this list contains only the prenames of the persons who attend at the event</returns>
        public List<string> getPrenamesStringArray(List<Rsvp> rsvpArray)
        {
            List<String> onlyPrenameList = new List<String>();
            String[] splitItem;

            //split the pre- and lastnames
            foreach (var item in rsvpArray)
            {
                splitItem = item.name.Split(new[] { " ", "-" }, StringSplitOptions.None);
                onlyPrenameList.Add(splitItem[0]);
            }
            return onlyPrenameList;
        }


        /// <summary>
        /// This method returns a the gender of a prename by:
        ///     a) Searching in th database 
        ///     b) Do a request to genderize.io
        /// </summary>
        /// <param name="name">prename of a person</param>
        /// <returns>gender of the prename</returns>
        public async Task<Gender> getGender(string name)
        {
            Gender gender;
            try
            {
                //search in database
                gender = await searchDbForGender(name);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                gender = null;
            }

            //gender==null => name is not in database => request to genderize.io needed
            if (gender == null)
            {
                //request to genderize.io
                gender = getGenderFromApi(name);
                //store new name and gender to database
                if (gender != null) pushGenderToDb(gender);

            }
            return gender;
        }

        /// <summary>
        /// GetGender uses the genderize.io API to get the gender of a prename
        /// </summary>
        /// <param name="name">prename of a person</param>
        /// <returns>gender of the prename</returns>
        public Gender getGenderFromApi(string name) {
            //if we still have to wait for our limit to go away just return null, we cant make any api calls at the moment anyways!
            if (DateTime.Now < xRateNextTry) return null;
            string result;
            Gender gender = null;

            var getData = "name=" + name;
            url = "http://api.genderize.io/?";
            var uri = new Uri(url + getData);
            var request = (HttpWebRequest)WebRequest.Create(uri);

            request.Method = "GET";

            request.AllowAutoRedirect = true;

            new UTF8Encoding();


            HttpWebResponse response;
            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            //get the limit of request we can do until we get blocked
                            xRateLimitRemaining = int.Parse(response.Headers["X-Rate-Limit-Remaining"]);
                            //get the time at which we can do the next request after we got blocked
                            xRateReset = int.Parse(response.Headers["X-Rate-Reset"]);
                            //we set a timer for 25 hours. subsequent calls to this method will be ignored until the timer has elapsed
                            if (xRateLimitRemaining < 2) xRateNextTry = DateTime.Now.AddHours(25);
                            lastRequest = DateTime.Now;

                            //String of the json from genderize.io
                            result = readStream.ReadToEnd();
                            //deserialize  json string to gender entitiy
                            gender = JsonConvert.DeserializeObject<Gender>(result);
                            //if genderize.io returns gender=null, than save gender as undifined
                            if (gender.gender == null)
                            {
                                gender.gender = "undefined";
                            }

                            return gender;
                        }
                    }
                }
            }
            catch (WebException webEx)
            {
                if (xRateLimitRemaining < 2) {
                    xRateNextTry = DateTime.Now.AddHours(25);
                    return null;
                }
                Debug.WriteLine("Error: " + webEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                if (gender != null)
                {
                    gender.name = name;
                    gender.gender = "undefined";
                    gender.count = 0;
                    gender.probability = 0;
                    return gender;
                }
                throw;
            }
        }

        /// <summary>
        /// Creates a statistik of the attending people of an event. This statisik contains the amount of
        /// males and females of the event. Undifined is only used if no gender can be found.
        /// </summary>
        /// <returns>event which include the three new values male, female and undifined</returns>
        public async Task<Event> createGenderStat(Event newEvent)
        {
            var male = 0;
            var female = 0;
            var undefined = 0;

            Gender gender;

            //get list of people attending the event
            var attendingList = newEvent.attending;
            // get the prename list of the attending people
            var preNameList = getPrenamesStringArray(attendingList);

            //create the statistik of prenames/ gender of this prename
            foreach (var name in preNameList)
            {
                gender = await getGender(name);
                if (gender == null)
                {
                    undefined++;
                    continue;
                }
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

            //save values to event
            newEvent.attendingMale = male;
            newEvent.attendingFemale = female;
            newEvent.attendingUndefined = undefined;

            return newEvent;
        }

        #region HelperMethods

        /// <summary>
        /// Stores the Gender in the database
        /// </summary>
        /// <param name="gender">object of gender</param>
        private async void pushGenderToDb(Gender gender)
        {
            try
            {
                //insert the gender to database
                await repoGender.InsertAsync(gender);
            }
            catch (MongoWriteException e)
            {
                //Console.Write(e.Message);
            }
            catch (MongoWaitQueueFullException ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                Thread.Sleep(15000);
                pushGenderToDb(gender);
            }
        }

        /// <summary>
        /// Searches in the database for the gender of a persons prename
        /// </summary>
        /// <param name="name">prename of a person</param>
        /// <returns>returns the gender from database</returns>
        public async Task<Gender> searchDbForGender(string name)
        {
            return await repoGender.GetByNameAsync(name);
        }

        #endregion
    }
}