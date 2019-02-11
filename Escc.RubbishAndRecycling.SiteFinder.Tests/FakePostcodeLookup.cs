using System.Threading.Tasks;
using Escc.Geo;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    /// <summary>
    /// A fake postcode lookup implementation which always returns the coordinates of County Hall
    /// </summary>
    public class FakePostcodeLookup : IPostcodeLookup
    {
        public Task<LatitudeLongitude> CoordinatesAtCentreOfPostcodeAsync(string postcode)
        {
            return Task.FromResult(new LatitudeLongitude(50.872066, 0.0010903126));
        }
    }
}