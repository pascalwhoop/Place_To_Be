using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
        public static DateTime getDateTimeFromISOString(String isoDate)
        {
            var date = DateTime.Parse(isoDate, null,
                System.Globalization.DateTimeStyles.RoundtripKind);
            return date;
        }

        /// <summary>
        /// Get the fbId of a fbUser from httpContext
        /// </summary>
        /// <param name="httpContext">HttpContext from request</param>
        /// <returns>fbId of the User who have done the request</returns>
        public String getFbIdFromHttpContext(HttpContext httpContext)
        {
            string authHeader = null;
            //get the Header from the HttpContext
            var auth = httpContext.Request.Headers["Authorization"];
            if (auth != null)
                authHeader = auth.Split(' ')[1]; //gives us the base 64 encoded string of the Basic Header

            if (string.IsNullOrEmpty(authHeader))
                return null;

            //convert from Base64 to String
            authHeader = Encoding.Default.GetString(Convert.FromBase64String(authHeader));

            var tokens = authHeader.Split(':');
            //check if string only contains numbers
            string regx = @"^[0-9]*$";
            if (Regex.IsMatch(tokens[0], regx))
            {

                return tokens[0];//fbId
            }
            return null;
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
                            Debug.WriteLine(ex.InnerException);
                            response = null;
                            break;

                        case HttpStatusCode.BadRequest:
                            response= null;
                            Debug.WriteLine(ex.InnerException);
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