using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace placeToBe.Services
{
    /// <summary>
    /// UtilService provides static helper methods that are often used by other methods
    /// (providing redundancy).
    /// </summary>
    public class UtilService
    {
        /// <summary>
        /// parses a ISO Date String into a DateTime Object. 
        /// </summary>
        /// <param name="isoDate">Date in ISO format</param>
        /// <returns>returns a DateTime Object</returns>
        public static DateTime getDateTimeFromISOString(String isoDate) {
            var date = DateTime.Parse(isoDate, null,
                System.Globalization.DateTimeStyles.RoundtripKind);
            return date;
        }
        /// <summary>
        /// Performs a Simple Web GET Request to the specified URL.
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <returns>returns the web response result as a string</returns>
        public static string performGetRequest(Uri uri)
        {
            String response = null;
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(uri);
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