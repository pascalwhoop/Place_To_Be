﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Security;

namespace placeToBe.Model.Entities
{
    /// <summary>
    /// User class containing all information for a users verfication.
    /// </summary>
    [DataContract]
    public class User : EntityBase
    {
        //The 0-parameter constructor is necessary for the structure of inheritance
        public User(){}
        public User(string email)
        {
            this.email = email;
        }
        public User(String email, byte[] passwordSalt, byte[] salt)
        {
            this.email = email;
            this.passwordSalt = passwordSalt;
            this.salt = salt;
        
        }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string password { get; set; }
        public byte[] passwordSalt { get; set; }
        public byte[] salt { get; set; }
       [DataMember]
        // if status == true then the user is activated/confirmed, else not activated/confirmed
        public bool status { get; set; }
       [DataMember]
        public string activationcode { get; set; }
       [DataMember]
        public string company { get; set; }
        [DataMember]
        public string city { get; set; }


    }

    public class PasswordChangePair {
        public string email { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }
}