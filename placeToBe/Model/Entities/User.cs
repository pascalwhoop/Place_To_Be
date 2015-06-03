using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace placeToBe.Model.Entities
{
    public class User: EntityBase
    {
        public User(String email, byte[] passwordSalt, byte[] salt)
        {
            this.email = email;
            this.passwordSalt = passwordSalt;
            this.salt = salt;
        }
        public string userId { get; set; }
        public string email { get; set; }
        public byte[] passwordSalt { get; set; }
        public byte[] salt { get; set; }
    }
    public class CityUser : User
    {
        public string city { get; set; }
    }
    public class CompanyUser : User
    {
        public string company { get; set; }        
    }



}