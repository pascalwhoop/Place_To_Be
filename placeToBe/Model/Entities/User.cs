using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace placeToBe.Model.Entities
{
    public class User: EntityBase
    {
        public string UserId { get; set; }
        public string email { get; set; }
        public byte[] passwordSalt { get; set; }

    }
}