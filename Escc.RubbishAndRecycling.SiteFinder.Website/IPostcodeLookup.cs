using Escc.Geo;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    /// <summary>
    /// A strategy to look up the central coordinates of a postcode area
    /// </summary>
    public interface IPostcodeLookup
    {
        /// <summary>
        /// Gets the coordinates at the centre of a postcode.
        /// </summary>
        /// <param name="postcode">The postcode.</param>
        /// <returns></returns>
        LatitudeLongitude CoordinatesAtCentreOfPostcode(string postcode);
    }
}