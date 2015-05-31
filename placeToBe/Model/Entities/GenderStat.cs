using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace placeToBe.Model.Entities
{
    public class GenderStat: EntityBase
    {
        public GenderStat(int male, int female)
        {
            this.male = male;
            this.female = female;
        }
        public int male { get; set; }
        public int female { get; set; }
    }
}