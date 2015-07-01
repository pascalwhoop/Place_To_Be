using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace placeToBe.Model.Entities
{
    public class FBUser : User
    {
        /// <summary>
        /// Faceboob Entity
        /// </summary>
        /// <param name="FB_ID"></param>
        /// <param name="emailFB"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="nickname"></param>
        /// <param name="gender"></param>
        /// <param name="httpLink"></param>
        /// <param name="country"></param>
        /// <param name="timezone"></param>
        /// <param name="updatedTimeFB"></param>
        /// <param name="verified"></param>
        public FBUser(int FB_ID, string emailFB, string firstName, string lastName, string nickname, string gender, string httpLink, string country, int timezone, string updatedTimeFB, bool verified)
        {
            this.FB_ID = FB_ID;
            this.emailFB = emailFB;
            this.firstName = firstName;
            this.lastName = lastName;
            this.nickname = nickname;
            this.gender = gender;
            this.httpLink = httpLink;
            this.country = country;
            this.timezone = timezone;
            this.updatedTimeFB = updatedTimeFB;
            this.verified = verified;
        }
        public int FB_ID { get; set; }
        public string emailFB { get; set; }
        public string firstName {get; set;}
        public string lastName { get; set; }
        public string nickname { get; set; }
        public string gender { get; set; }
        public string httpLink { get; set; }
        public string country { get; set; }
        public int timezone { get; set; }
        public string updatedTimeFB { get; set; }
        public bool verified { get; set; }

    }
}