using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
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
        private readonly IProxyProvider _proxyProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocateApiPostcodeLookup" /> class.
        /// </summary>
        /// <param name="locateApiUrl">The locate API URL.</param>
        /// <param name="authenticationToken">The authentication token.</param>
        /// <param name="proxyProvider">The proxy provider.</param>
        /// <exception cref="System.ArgumentNullException">authenticationToken</exception>
        public LocateApiPostcodeLookup(Uri locateApiUrl, string authenticationToken, IProxyProvider proxyProvider)
        {
            if (locateApiUrl == null) throw new ArgumentNullException(nameof(locateApiUrl));
            if (String.IsNullOrEmpty(authenticationToken)) throw new ArgumentNullException(nameof(authenticationToken));

            _locateApiUrl = locateApiUrl;
            _authenticationToken = authenticationToken;
            _proxyProvider = proxyProvider;
        }

        /// <summary>
        /// Gets the coordinates at the centre of a postcode, based on the mean position of addresses rather than the geographic centre
        /// </summary>
        /// <param name="postcode">The postcode.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public LatitudeLongitude CoordinatesAtCentreOfPostcode(string postcode)
        {
            var query = Regex.Replace(postcode, "[^A-Za-z0-9]", String.Empty);
            if (String.IsNullOrEmpty(query)) return null;

            try
            {
                using (var client = new WebClient())
                {
                    if (_proxyProvider != null)
                    {
                        client.Proxy = _proxyProvider.CreateProxy();
                    }
                    client.Headers.Add("Authorization", "Bearer " + _authenticationToken);

                    var queryUrl = String.Format(_locateApiUrl.ToString(), query);

                    using (var stream = new StreamReader(client.OpenRead(queryUrl)))
                    {
                        var json = stream.ReadToEnd();
                        var result = JsonConvert.DeserializeObject<LocateApiResult>(json);
                        return new LatitudeLongitude(result.latitude, result.longitude);
                    }
                }
            }
            catch (WebException exception)
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