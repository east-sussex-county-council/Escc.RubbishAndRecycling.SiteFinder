using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Escc.Geo;
using Escc.Net;
using Exceptionless;
using Newtonsoft.Json;

namespace Escc.RubbishAndRecycling.SiteFinder.Website
{
    /// <summary>
    /// Looks up a postcode using an implementation of the GOV.UK locate API defined at https://github.com/alphagov/locate-api
    /// </summary>
    /// <seealso cref="IPostcodeLookup" />
    public class LocateApiPostcodeLookup : IPostcodeLookup
    {
        private readonly Uri _locateApiUrl;
        private readonly string _authenticationToken;
        private readonly IHttpClientProvider _httpClientProvider;
        private static HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocateApiPostcodeLookup" /> class.
        /// </summary>
        /// <param name="locateApiUrl">The locate API URL.</param>
        /// <param name="authenticationToken">The authentication token.</param>
        /// <param name="httpClientProvider">A way to get a shared instance of an <see cref="HttpClient"/>.</param>
        /// <exception cref="System.ArgumentNullException">authenticationToken</exception>
        public LocateApiPostcodeLookup(Uri locateApiUrl, string authenticationToken, IHttpClientProvider httpClientProvider)
        {
            if (String.IsNullOrEmpty(authenticationToken)) throw new ArgumentNullException(nameof(authenticationToken));

            _locateApiUrl = locateApiUrl ?? throw new ArgumentNullException(nameof(locateApiUrl));
            _authenticationToken = authenticationToken;
            _httpClientProvider = httpClientProvider ?? throw new ArgumentNullException(nameof(httpClientProvider));
        }

        /// <summary>
        /// Gets the coordinates at the centre of a postcode, based on the mean position of addresses rather than the geographic centre
        /// </summary>
        /// <param name="postcode">The postcode.</param>
        /// <returns></returns>
        public async Task<LatitudeLongitude> CoordinatesAtCentreOfPostcodeAsync(string postcode)
        {
            var query = Regex.Replace(postcode, "[^A-Za-z0-9]", String.Empty);
            if (String.IsNullOrEmpty(query)) return null;

            try
            {
                if (_httpClient == null)
                {
                    _httpClient = _httpClientProvider.GetHttpClient();
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationToken);
                }

                var queryUrl = String.Format(_locateApiUrl.ToString(), query);
                var json = await _httpClient.GetStringAsync(queryUrl);
                var result = JsonConvert.DeserializeObject<LocateApiResult>(json);
                return new LatitudeLongitude(result.latitude, result.longitude);
            }
            catch (HttpRequestException exception)
            {
                if (!exception.Message.Contains("(422) Unprocessable Entity"))
                {
                    exception.ToExceptionless().Submit();
                }
                return null;
            }
        }
    }
}