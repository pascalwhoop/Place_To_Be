using System.Collections.Generic;

namespace placeToBe.Model.Entities {
    /// <summary>
    /// FacebookUserVerification helper class to deserialise Facebooks JSON objects to C# objects.
    /// </summary>
    public class FbTokenInspection {
        public FbInspectionData data { get; set; }

    }

    public class FbInspectionData
    {
        public string app_id { get; set; }
        public string application { get; set; }
        public int expires_at { get; set; }
        public bool is_valid { get; set; }
        public List<string> scopes { get; set; }
        public string user_id { get; set; }
    }

}