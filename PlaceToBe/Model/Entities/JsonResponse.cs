using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace placeToBe.Model.Entities
{
    /// <summary>
    /// Helper Class to deserialise JSON Responses to C# objects
    /// </summary>
    public class JsonResponse
    {
        string status { get; set; }
        string message { get; set; }
    }
}