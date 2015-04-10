using System;
using Escc.Exceptions.Soap;
using Escc.FormControls.WebForms.AddressFinder;
using Escc.Geo;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    /// <summary>
    /// Look up the central coordinates of a postcode using a web service that queries GIS data
    /// </summary>
    public class PostcodeLookupWebService : IPostcodeLookup
    {
        /// <summary>
        /// Gets the coordinates at the centre of a postcode.
        /// </summary>
        /// <param name="postcode">The postcode.</param>
        /// <returns></returns>
        public LatitudeLongitude CoordinatesAtCentreOfPostcode(string postcode)
        {
            using (var af = new AddressFinder())
            {
                try
                {
                    var eastingAndNorthing = af.AggregateEastingsAndNorthings(postcode);
                    var converter = new OrdnanceSurveyToLatitudeLongitudeConverter();
                    return converter.ConvertOrdnanceSurveyToLatitudeLongitude(eastingAndNorthing.Easting, eastingAndNorthing.Northing);
                }
                catch (Exception ex)
                {
                    throw SoapExceptionEngine.GetSoapException(ex.Message);
                }
            }
        }
    }
}