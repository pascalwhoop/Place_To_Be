using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace placeToBe.Service
{
    /**
     * a class that contains some static utility stuff
     * 
     * http://stackoverflow.com/questions/9453101/how-do-i-get-epoch-time-in-c
     * http://stackoverflow.com/questions/3556144/how-to-create-a-net-datetime-from-iso-8601-format
     */
    public class UtilService
    {
        public static DateTime getDateTimeFromISOString(String isoDate) {
            var date = DateTime.Parse(isoDate, null,
                System.Globalization.DateTimeStyles.RoundtripKind);
            return date;
        }
    }
}