using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace placeToBe.Services
{
    /**
     * a class that contains some static utility stuff
     * 
     * http://stackoverflow.com/questions/9453101/how-do-i-get-epoch-time-in-c
     * http://stackoverflow.com/questions/3556144/how-to-create-a-net-datetime-from-iso-8601-format
     */
    public class UtilService
    {

        /**
         * parses a ISO Date String into a DateTime Object. 
         */
        public static DateTime getDateTimeFromISOString(String isoDate) {
            var date = DateTime.Parse(isoDate, null,
                System.Globalization.DateTimeStyles.RoundtripKind);
            return date;
        }

        /**
         * Performs a Simple Web GET Request to the specified URL and returns the result as a string
         */
        public static string performGetRequest(Uri URI)
        {
            String response = null;
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(URI);
                if (stream != null)
                {
                    StreamReader reader = new StreamReader(stream);
                    response = reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse)
                {
                    switch (((HttpWebResponse)ex.Response).StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            response = null;
                            break;

                        default:
                            throw ex;
                    }

                }
            }
            return response;

        }

    }
}