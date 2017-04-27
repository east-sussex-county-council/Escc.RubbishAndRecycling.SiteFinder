using Newtonsoft.Json;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    /// <summary>
    /// The decoded JSON result of a query to a locate API query
    /// </summary>
    public class LocateApiResult
    {
        public string gssCode { get; set; }
        public string country { get; set; }
        public string postcode { get; set; }
        public string name { get; set; }
        public float easting { get; set; }
        public float northing { get; set; }
        [JsonProperty(PropertyName ="lat")]
        public float latitude { get; set; }
        [JsonProperty(PropertyName ="long")]
        public float longitude { get; set; }
        public string nhsRegionalHealthAuthority { get; set; }
        public string nhsHealthAuthority { get; set; }
        public string county { get; set; }
        public string ward { get; set; }
    }
}