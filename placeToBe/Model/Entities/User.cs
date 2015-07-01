using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace placeToBe.Model.Entities
{
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
        public string email { get; set; }
        public byte[] passwordSalt { get; set; }
        public byte[] salt { get; set; }
        // if status == true then the user is activated/confirmed, else not activated/confirmed
        public bool status { get; set; }
        public string activationcode { get; set; }
        public string company { get; set; }
        public string city { get; set; }

    }
}